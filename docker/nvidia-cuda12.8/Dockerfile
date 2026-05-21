# Use the official NVIDIA CUDA 12.8 Development image
FROM nvidia/cuda:12.8.0-devel-ubuntu22.04

# Prevent interactive prompts
ENV DEBIAN_FRONTEND=noninteractive

# Install essential build tools and dependencies for .NET
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
    cudnn9-cuda-12 \
    libicu70 \
    && rm -rf /var/lib/apt/lists/*

# Install .NET 10.0 using the official script
# We install it to /usr/share/dotnet so it's available globally
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 10.0 --install-dir /usr/share/dotnet

# Set Environment Variables so the system finds 'dotnet'
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools

# Verify dotnet installation
RUN dotnet --version

# Install vcpkg
RUN git clone https://github.com/microsoft/vcpkg.git /opt/vcpkg && \
    /opt/vcpkg/bootstrap-vcpkg.sh -disableMetrics
ENV VCPKG_INSTALLATION_ROOT=/opt/vcpkg

RUN git config --global --add safe.directory /repo
WORKDIR /repo