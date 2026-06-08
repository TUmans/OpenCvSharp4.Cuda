FROM ubuntu:24.04

ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get install -y \
    build-essential \
    cmake \
    ninja-build \
    git \
    curl \
    zip \
    unzip \
    tar \
    pkg-config \
    nasm \
    libtesseract-dev \
    libleptonica-dev \
    libicu74 \
    wget \
    ca-certificates \
    libavcodec-dev \
    libavformat-dev \
    libavutil-dev \
    libswscale-dev \
    libavdevice-dev \
    libgtk2.0-dev \
    libva-dev \
    libvdpau-dev \
    && rm -rf /var/lib/apt/lists/*

RUN wget https://developer.download.nvidia.com/compute/cuda/repos/ubuntu2404/x86_64/cuda-keyring_1.1-1_all.deb \
    && dpkg -i cuda-keyring_1.1-1_all.deb \
    && rm cuda-keyring_1.1-1_all.deb \
    && apt-get update && apt-get install -y \
    cuda-toolkit-12-8 \
    cuda-libraries-dev-12-8 \
    libcudnn9-dev-cuda-12 \
    && rm -rf /var/lib/apt/lists/*

# NVIDIA Video Codec SDK - headers
RUN mkdir -p /opt/video_codec_sdk
COPY extern/Video_Codec_SDK_13.0.37 /opt/video_codec_sdk/
COPY extern/Video_Codec_SDK_13.0.37/Interface/* /usr/local/cuda-12.8/include/
COPY extern/Video_Codec_SDK_13.0.37/Lib/linux/stubs/x86_64/* /usr/local/cuda-12.8/lib64/stubs/

RUN ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so      /usr/local/cuda-12.8/lib64/stubs/nvcuvid.so \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libcuda.so          /usr/local/cuda-12.8/lib64/stubs/libcuda.so.1 \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so      /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so.1 \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so /usr/local/cuda-12.8/lib64/stubs/nvidia-encode.so \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so.1

ENV PATH=/usr/local/cuda-12.8/bin:${PATH:-}
ENV LD_LIBRARY_PATH=/usr/local/cuda-12.8/lib64:/usr/local/cuda-12.8/lib64/stubs:${LD_LIBRARY_PATH:-}
ENV LIBRARY_PATH=/usr/local/cuda-12.8/lib64/stubs:${LIBRARY_PATH:-}
ENV CPATH=/usr/local/cuda-12.8/include:${CPATH:-}

ENV CPATH=/usr/local/cuda-12.8/include:$CPATH 

RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
RUN dotnet --version

RUN git clone https://github.com/microsoft/vcpkg.git /opt/vcpkg && \
    /opt/vcpkg/bootstrap-vcpkg.sh -disableMetrics
ENV VCPKG_INSTALLATION_ROOT=/opt/vcpkg

RUN git config --global --add safe.directory /repo
WORKDIR /repo