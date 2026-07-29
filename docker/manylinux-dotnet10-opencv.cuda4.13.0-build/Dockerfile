FROM quay.io/pypa/manylinux_2_28_x86_64

ENV DEBIAN_FRONTEND=noninteractive

# Enable RPM Fusion for FFmpeg
RUN dnf install -y \
    https://dl.fedoraproject.org/pub/epel/epel-release-latest-8.noarch.rpm \
    && dnf install -y \
    https://mirrors.rpmfusion.org/free/el/rpmfusion-free-release-8.noarch.rpm \
    https://mirrors.rpmfusion.org/nonfree/el/rpmfusion-nonfree-release-8.noarch.rpm \
    && dnf clean all

# Basic build tools + FFmpeg dev libs
RUN dnf install -y \
    gcc-c++ \
    ninja-build \
    git \
    zip \
    unzip \
    nasm \
    wget \
    perl-IPC-Cmd \
    perl-Time-Piece \
    gtk2-devel \
    libva-devel \
    libvdpau-devel \
    && dnf clean all

# Activate gcc-toolset-14 for all subsequent commands
ENV PATH=/opt/rh/gcc-toolset-14/root/usr/bin:${PATH}
ENV LD_LIBRARY_PATH=/opt/rh/gcc-toolset-14/root/usr/lib64:${LD_LIBRARY_PATH}
ENV CC=/opt/rh/gcc-toolset-14/root/usr/bin/gcc
ENV CXX=/opt/rh/gcc-toolset-14/root/usr/bin/g++

# CUDA repo for RHEL8/AlmaLinux8
RUN dnf config-manager --add-repo https://developer.download.nvidia.com/compute/cuda/repos/rhel8/x86_64/cuda-rhel8.repo \
    && dnf install -y \
    cuda-toolkit-12-8 \
    cuda-libraries-devel-12-8 \
    libcudnn9-devel-cuda-12 \
    && dnf clean all

# NVIDIA Video Codec SDK - headers
RUN mkdir -p /opt/video_codec_sdk
COPY extern/Video_Codec_SDK_13.0.37 /opt/video_codec_sdk/
COPY extern/Video_Codec_SDK_13.0.37/Interface/* /usr/local/cuda-12.8/include/
COPY extern/Video_Codec_SDK_13.0.37/Lib/linux/stubs/x86_64/* /usr/local/cuda-12.8/lib64/stubs/

RUN ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so      /usr/local/cuda-12.8/lib64/stubs/nvcuvid.so \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libcuda.so       /usr/local/cuda-12.8/lib64/stubs/libcuda.so.1 \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so    /usr/local/cuda-12.8/lib64/stubs/libnvcuvid.so.1 \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so /usr/local/cuda-12.8/lib64/stubs/nvidia-encode.so \
    && ln -sf /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so /usr/local/cuda-12.8/lib64/stubs/libnvidia-encode.so.1

ENV PATH=/usr/local/cuda-12.8/bin:${PATH}
ENV LD_LIBRARY_PATH=/usr/local/cuda-12.8/lib64:/usr/local/cuda-12.8/lib64/stubs
ENV LIBRARY_PATH=/usr/local/cuda-12.8/lib64/stubs
ENV CPATH=/usr/local/cuda-12.8/include

# .NET 10
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir /usr/share/dotnet
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools
RUN dotnet --version

# vcpkg
RUN git clone https://github.com/microsoft/vcpkg.git /opt/vcpkg && \
    /opt/vcpkg/bootstrap-vcpkg.sh -disableMetrics
ENV VCPKG_INSTALLATION_ROOT=/opt/vcpkg

RUN git config --global --add safe.directory /repo
WORKDIR /repo