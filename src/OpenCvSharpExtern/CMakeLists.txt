cmake_minimum_required(VERSION 3.15)
project(OpenCvSharpExtern LANGUAGES CXX C)

set(REPO_ROOT "${CMAKE_CURRENT_SOURCE_DIR}/../..")

# --- PATH FIX ---
if(UNIX)
    if("${VCPKG_INSTALLED_DIR}" MATCHES "^[A-Z]:")
        set(VCPKG_INSTALLED_DIR "/repo/vcpkg_installed_linux")
        set(VCPKG_TARGET_TRIPLET "x64-linux-static")
    endif()
    set(VCPKG_LIB_PATH "${VCPKG_INSTALLED_DIR}/${VCPKG_TARGET_TRIPLET}/lib")
    link_directories(${VCPKG_LIB_PATH})
endif()

# --- ICONV BRIDGE ---
if(UNIX)
    find_package(Iconv REQUIRED)
    if(NOT TARGET Iconv::Iconv)
        add_library(Iconv::Iconv INTERFACE IMPORTED)
        set_target_properties(Iconv::Iconv PROPERTIES
            INTERFACE_INCLUDE_DIRECTORIES "${Iconv_INCLUDE_DIRS}"
            INTERFACE_LINK_LIBRARIES "${Iconv_LIBRARIES}"
        )
    endif()
endif()

# --- FIND OPENCV ---
if(WIN32)
    set(OpenCV_STATIC ON CACHE BOOL "")
    set(OpenCV_RUNTIME "vc17" CACHE STRING "")
    set(OpenCV_ARCH "x64" CACHE STRING "")
endif()
find_package(OpenCV REQUIRED)

# --- SOURCE COLLECTION (Explicit & Deduplicated) ---
include_directories(${OpenCV_INCLUDE_DIRS})
include_directories("${REPO_ROOT}/extern/OpenCvSharp/src/OpenCvSharpExtern")
include_directories("${CMAKE_CURRENT_SOURCE_DIR}") 

file(GLOB BASE_FILES "${REPO_ROOT}/extern/OpenCvSharp/src/OpenCvSharpExtern/*.cpp")
file(GLOB OVERRIDE_FILES "${CMAKE_CURRENT_SOURCE_DIR}/*.cpp")

set(FINAL_SRC_LIST "")
foreach(F ${BASE_FILES})
    get_filename_component(N "${F}" NAME)
    set(IS_OVERRIDDEN FALSE)
    foreach(O ${OVERRIDE_FILES})
        get_filename_component(ON "${O}" NAME)
        if(N STREQUAL ON)
            set(IS_OVERRIDDEN TRUE)
        endif()
    endforeach()
    if(NOT IS_OVERRIDDEN)
        list(APPEND FINAL_SRC_LIST "${F}")
    endif()
endforeach()
list(APPEND FINAL_SRC_LIST ${OVERRIDE_FILES})

# --- TARGET ---
add_library(OpenCvSharpExtern SHARED ${FINAL_SRC_LIST})

# --- COMPILER FLAGS (Force Visibility) ---
if(UNIX)
    # Force symbols to be visible so .NET can see them
    target_compile_options(OpenCvSharpExtern PRIVATE -fvisibility=default)
    set_target_properties(OpenCvSharpExtern PROPERTIES 
        CXX_VISIBILITY_PRESET default
        VISIBILITY_INLINES_HIDDEN OFF)
endif()

# --- LINKING ---
target_link_libraries(OpenCvSharpExtern PRIVATE ${OpenCV_LIBRARIES})

if(UNIX)
    # Dependency absolute paths
    find_library(TESS_LIB NAMES tesseract PATHS ${VCPKG_LIB_PATH} NO_DEFAULT_PATH)
    if(TESS_LIB)
        target_link_libraries(OpenCvSharpExtern PRIVATE ${TESS_LIB})
    endif()
    
    find_library(HARF_LIB NAMES harfbuzz PATHS ${VCPKG_LIB_PATH} NO_DEFAULT_PATH)
    if(HARF_LIB)
        target_link_libraries(OpenCvSharpExtern PRIVATE ${HARF_LIB})
    endif()

    # Leptonica: OCRTesseract calls Leptonica functions (pixaCreate, etc.) directly,
    # not just through Tesseract's own API, so it must be linked explicitly here too.
    # Without this, the symbol was only ever resolved on dev machines that happened
    # to have a stray system liblept installed - a clean runtime image fails with
    # "undefined symbol: pixaCreate" at load time.
    find_library(LEPT_LIB NAMES leptonica lept PATHS ${VCPKG_LIB_PATH} NO_DEFAULT_PATH)
    if(LEPT_LIB)
        target_link_libraries(OpenCvSharpExtern PRIVATE ${LEPT_LIB})
    endif()

    # libarchive: Tesseract can link against it (for reading compressed/bundled
    # traineddata), same masking problem as tesseract/harfbuzz/leptonica above -
    # a stray system liblarchive hid this on dev machines. Undefined symbol was
    # "archive_read_new".
    find_library(ARCHIVE_LIB NAMES archive PATHS ${VCPKG_LIB_PATH} NO_DEFAULT_PATH)
    if(ARCHIVE_LIB)
        target_link_libraries(OpenCvSharpExtern PRIVATE ${ARCHIVE_LIB})
    endif()

    # Linker Flags: exclude-libs is still good (it hides OpenCV internals, but NOT your wrapper)
    set(PORTABLE_LINKER_FLAGS "-Wl,--exclude-libs,ALL -static-libgcc -static-libstdc++")
    target_link_options(OpenCvSharpExtern PRIVATE ${PORTABLE_LINKER_FLAGS})
    target_link_libraries(OpenCvSharpExtern PRIVATE dl pthread rt m)
endif()

install(TARGETS OpenCvSharpExtern DESTINATION lib)