# STM32F051 Discovery Board Setup

## Overview
This guide covers setting up **STM32CubeIDE** for the **STM32F051 Discovery Board**, including **UART** configuration.

## Prerequisites
Ensure you have:
- [STM32CubeIDE](https://www.st.com/en/development-tools/stm32cubeide.html)
- STM32F051 Discovery Board
- USB Debugger (ST-Link V2 integr.)
- Serial terminal software (e.g., PuTTY, Tera Term)

## Setup Steps
### 1. Create a New STM32CubeIDE Project
1. Open **STM32CubeIDE**.
2. Go to **File** -> **New** -> **STM32 Project**.
3. Select **STM32F051R8**, click **Next**, name your project, and select **CubeIDE**.
4. Click **Finish**.

### 2. Configure Peripherals
1. Open **Pinout & Configuration**.
2. Enable **USART2** and assign **TX (PA2)** and **RX (PA3)**.
3. Click **Project Manager**, name your project, and generate code.

![image](https://github.com/user-attachments/assets/ea49912c-6125-4e35-ad45-05447cc51d79)
> Figure 2. Confguration of pins

### 3. Build & Flash the Project
1. Click **Project** -> **Build All**.
2. Connect the board via USB.
3. Go to **Run** -> **Run Configurations...**, select **STM32 Cortex-M C/C++ Application**, and configure the debugger.
4. Click **Run** to flash the program.

### 4. Test UART Communication
1. Open a serial terminal and select the correct COM port.
2. Set baud rate to **115200**.

## Conclusion
You have successfully set up **STM32CubeIDE** for the **STM32F051 Discovery Board*
