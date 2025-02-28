## Signal Generator
---

## [Technical Task](technical_task.md)

---

# 1. Overview

This document describes the communication protocol for configuring and controlling the pulse generator based on STM32. The protocol defines the structure of commands sent from a PC to the STM32 via UART and the responses sent back by the STM32.

# 2. [Envoriment Setup](env_setup.md)

# 3 Develop and document communication protocol 

## 3.1 Communication Protocol Description

Communication between the STM32F051R8T6 microcontroller and a personal computer will be carried out using asynchronous UART/USART transmission. This allows setting signal parameters and obtaining information about the current state of the generator.

The structural diagram of data transmission is shown in Figure 1.1.
![image](https://github.com/user-attachments/assets/bb0f9d32-b3bd-48e2-90ae-bb6ef52ec5ab)

Figure 1.1 – Structural Diagram of Communication between the STM32F051R8T6 Microcontroller and the Personal Computer

Since the STM32F051 Discovery Board does not have a built-in USB/UART converter, an external module is used to establish a connection between the microcontroller and the personal computer, as depicted in Figure 1.1.

On the personal computer, a user interface window will display input/output data and a virtual COM port connection window.

## 3.2 Message and Data Packet Structure

To generate rectangular pulses, a command is sent from the user terminal, which is converted into a binary or hexadecimal code combination and transmitted to the STM32F051R8T6 microcontroller via the UART protocol.

The proposed message format is as follows:

█'N''S' 'x' 'y' 'check'

Where:

N – Signal number (since the project is designed for possible expansion to support different signal forms, each form will have its own sequential number). In the current setup, only one type of signal is used, assigned number 1.

S – Initial state level of the rectangular signal (1 for high level, 0 for low level).

x – Duration of the low-level period (in microseconds).

y – Duration of the high-level period (in microseconds).

check – Checksum, calculated as the sum of all previous parameters. This parameter is computed separately in both the user interface software and the STM32F051R8T6 microcontroller firmware.

Example of a Command:

█('1' '0' '1000' '500' '1501')

Based on this command, the microcontroller will generate a rectangular pulse sequence:

The first signal is formed.

The sequence starts with a low level lasting 1000 µs.

Followed by a high level lasting 500 µs.

Thus, the period of this signal is x + y, and the duty cycle is given by the ratio y / (x + y).

The waveform corresponding to command (1.2) is shown in Figure 1.2.
![image](https://github.com/user-attachments/assets/e10348c9-cf76-4e91-acbc-06e967b4b08b)

Figure 1.2 – Rectangular Signal Generation for the Given Command

The microcontroller receives this command as a packet of hexadecimal messages (e.g., 0x01 0x00 0x00 for generating a PWM signal). Upon successful reception, the microcontroller responds with a confirmation message such as:

Signal generate successful

This communication protocol ensures reliable and structured data exchange between the user interface and the STM32F051R8T6 microcontroller for generating precise pulse sequences.

Expanding error messages for detailed diagnostics.

Supporting additional waveform types.

This protocol ensures structured communication between the PC and STM32 for precise control of pulse generation.
