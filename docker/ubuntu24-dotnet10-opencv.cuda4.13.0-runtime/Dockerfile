FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
USER root
WORKDIR /app

# Added libx264-dev and libx265-dev for CPU fallbacks / FFmpeg HEVC support
RUN apt-get update && apt-get install -y --no-install-recommends \
    libgomp1 \
    libglib2.0-0 \
    libsm6 \
    libice6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libfontconfig1 \
    libfreetype6 \
    libharfbuzz0b \
    libjpeg-turbo8 \
    libpng16-16 \
    libtiff6 \
    libwebp7 \
    libtesseract5 \
    tesseract-ocr \
    ffmpeg \
    libx264-dev \
    libx265-dev \
    libatomic1 \
    libgdiplus \
    ca-certificates \
    libgtk2.0-0 \
    libwebp-dev \
    wget \
    && rm -rf /var/lib/apt/lists/*

# CUDA runtime libs (no toolkit, no dev headers)
RUN wget https://developer.download.nvidia.com/compute/cuda/repos/ubuntu2404/x86_64/cuda-keyring_1.1-1_all.deb \
    && dpkg -i cuda-keyring_1.1-1_all.deb \
    && rm cuda-keyring_1.1-1_all.deb \
    && apt-get update && apt-get install -y --no-install-recommends \
    cuda-cudart-12-8 \
    libcublas-12-8 \
    libcufft-12-8 \
    libcurand-12-8 \
    libcusolver-12-8 \
    libcusparse-12-8 \
    libnpp-12-8 \
    libcudnn9-cuda-12 \
    && rm -rf /var/lib/apt/lists/*

# EXPOSE VIDEO CAPABILITIES SO NVENC (Hardware Encoding) WORKS IN DOCKER
ENV NVIDIA_DRIVER_CAPABILITIES=compute,utility,video
ENV LD_LIBRARY_PATH=/usr/local/cuda/lib64:$LD_LIBRARY_PATH