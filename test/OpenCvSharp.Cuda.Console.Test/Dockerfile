# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# These ARGs allow for swapping out the base used to make the final image when debugging from VS
ARG LAUNCHING_FROM_VS
# This sets the base image for final, but only if LAUNCHING_FROM_VS has been defined
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER root
WORKDIR /app

# 1. NO TESSERACT INSTALLED HERE AT ALL
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgomp1 libglib2.0-0 libsm6 libice6 libx11-6 libxext6 libxrender1 \
    libfontconfig1 libfreetype6 libatomic1 libgdiplus ca-certificates \
    libgtk2.0-0 wget \
    && rm -rf /var/lib/apt/lists/*

# 2. TESSERACT FATAL CRASH FIX (Download the raw data model only)
# This proves your library is portable—it just needs the AI weights!
RUN mkdir -p /usr/share/tessdata && \
    wget -q https://github.com/tesseract-ocr/tessdata_fast/raw/main/eng.traineddata -O /usr/share/tessdata/eng.traineddata

ENV LC_ALL=C.UTF-8
ENV LANG=C.UTF-8
ENV TESSDATA_PREFIX=/usr/share/tessdata/

# 3. CUDA RUNTIME
RUN wget https://developer.download.nvidia.com/compute/cuda/repos/ubuntu2404/x86_64/cuda-keyring_1.1-1_all.deb \
    && dpkg -i cuda-keyring_1.1-1_all.deb \
    && rm cuda-keyring_1.1-1_all.deb \
    && apt-get update && apt-get install -y --no-install-recommends \
    cuda-cudart-12-8 libcublas-12-8 libcufft-12-8 libcurand-12-8 \
    libcusolver-12-8 libcusparse-12-8 libnpp-12-8 libcudnn9-cuda-12 \
    && rm -rf /var/lib/apt/lists/*

ENV NVIDIA_DRIVER_CAPABILITIES=compute,utility,video
ENV LD_LIBRARY_PATH=/usr/local/cuda/lib64:$LD_LIBRARY_PATH

USER $APP_UID


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["test/OpenCvSharp.Cuda.Console.Test/OpenCvSharp.Cuda.Console.Test.csproj", "test/OpenCvSharp.Cuda.Console.Test/"]
COPY ["src/OpenCvSharp.Cuda/OpenCvSharp.Cuda.csproj", "src/OpenCvSharp.Cuda/"]
RUN dotnet restore "./test/OpenCvSharp.Cuda.Console.Test/OpenCvSharp.Cuda.Console.Test.csproj"
COPY . .
WORKDIR "/src/test/OpenCvSharp.Cuda.Console.Test"
RUN dotnet build "./OpenCvSharp.Cuda.Console.Test.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OpenCvSharp.Cuda.Console.Test.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

# This stage is used as the base for the final stage when launching from VS to support debugging in regular mode (Default when not using the Debug configuration)
FROM base AS aotdebug
USER root
# Install GDB to support native debugging
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    gdb
USER app

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM ${FINAL_BASE_IMAGE:-mcr.microsoft.com/dotnet/runtime-deps:10.0} AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV NVIDIA_DRIVER_CAPABILITIES=compute,utility,video
ENV LD_LIBRARY_PATH=/usr/lib/x86_64-linux-gnu:/usr/local/cuda/lib64:/usr/local/cuda/targets/x86_64-linux/lib:/app
ENV LD_DEBUG=libs
ENTRYPOINT ["./OpenCvSharp.Cuda.Console.Test"]