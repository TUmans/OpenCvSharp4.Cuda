set(VCPKG_TARGET_ARCHITECTURE x64)
set(VCPKG_CRT_LINKAGE static)
set(VCPKG_LIBRARY_LINKAGE static)
set(VCPKG_CXX_FLAGS "/std:c++17")
set(VCPKG_C_FLAGS "")
set(VCPKG_PLATFORM_TOOLSET "v143")
# Release-only build: skip debug libraries to reduce build time and artifact size.
set(VCPKG_BUILD_TYPE release)
