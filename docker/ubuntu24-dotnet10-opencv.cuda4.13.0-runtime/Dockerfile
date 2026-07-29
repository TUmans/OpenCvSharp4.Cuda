FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base
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