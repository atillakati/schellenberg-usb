#!/bin/bash
set -e

if dpkg -s libusb-1.0-0-dev >/dev/null 2>&1; 
then
    echo "libusb-1.0-0-dev is already installed"
else
    echo "libusb-1.0-0-dev is not installed"    

    # Install libusb-1.0-0-dev
    apt-get update && apt-get install -y libusb-1.0-0-dev
    apt-get clean 
    rm -rf /var/lib/apt/lists/*

    echo "/usr/local/lib" >> /etc/ld.so.conf && ldconfig
fi

# Execute the main command
exec "$@"