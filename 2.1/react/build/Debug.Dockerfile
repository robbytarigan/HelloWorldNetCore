FROM microsoft/dotnet:2.1-sdk AS build

# Install Node
ENV NODE_VERSION 8.11.3
ENV NODE_DOWNLOAD_SHA 1ea408e9a467ed4571730e160993f67a100e8c347f6f9891c9a83350df2bf2be
RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

# Install the SSHD server
RUN apt-get update \
  && apt-get install -y --no-install-recommends openssh-server \
  && mkdir -p /run/sshd \
  && echo "root:Docker!" | chpasswd

#Copy settings file. See elsewhere to find them. 
COPY sshd_config /etc/ssh/sshd_config

COPY authorized_keys  root/.ssh/authorized_keys

# Install Visual Studio Remote Debugger
RUN apt-get install zip unzip

RUN curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l ~/vsdbg

EXPOSE 2222