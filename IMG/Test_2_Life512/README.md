# Conway's Game of Life in a Bootloader

https://github.com/fluffninja/life512


# Install nasm

```
sudo apt update
sudo apt install build-essential
wget https://www.nasm.us/pub/nasm/releasebuilds/2.15.05/nasm-2.15.05.tar.gz
tar -xvf nasm-2.15.05.tar.gz
cd nasm-2.15.05
./configure
make
sudo make install
```

# make img

```
cd ..
cd life512
make all
```


## Running

Install `qemu-system-i386` (tested with version `7.2.90`), then run using:

```sh
make run
```
