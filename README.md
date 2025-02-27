## Signal Generator
---

## [Technical Task](technical_task.md)

---

# 1. Overview

This document describes the communication protocol for configuring and controlling the pulse generator based on STM32. The protocol defines the structure of commands sent from a PC to the STM32 via UART and the responses sent back by the STM32.

# 2. [Envoriment Setup](env_setup.md)

# 3. Command Format

Each command follows a specific format:
```
 'N''S''x''y''ch'
```
Where:

- N (Number): Sequence number of the signal. Always set to 1.

- S (State): Initial level of the rectangular signal.

- 0 - Starts from LOW level.

- 1 - Starts from HIGH level.

- x (High Duration): Duration of the HIGH level in microseconds.

- y (Low Duration): Duration of the LOW level in microseconds.

- ch (Channel): Channel number (reserved for future implementation).

Example command:
```
'1''0''500''500'1'
```
This sets up a signal starting from LOW, with a 500 µs HIGH duration and 500 µs LOW duration.

# 4. Communication Sequences

## 4.1. PC to STM32 (Command Transmission)

The PC sends a command string following the defined format over UART.

## 4.2. STM32 to PC (Acknowledgment and Error Handling)

On successful command reception and parameter validation, STM32 responds with:
```
ACK
```
If the received command has invalid parameters, STM32 responds with:
```
ERR: Invalid Parameter
```
If an unknown command is received, STM32 responds with:
```
ERR: Unknown Command
```
# 5. Error Handling

## 5.1 Invalid Parameters

If any parameter exceeds its allowable range, STM32 returns an error message.

## 5.2 UART Communication Errors

If a UART error occurs (parity error, framing error, etc.), the STM32 will discard the received data and send:
```
ERR: UART Failure
```
# 6. Future Enhancements

Implementing multiple channel support.

Expanding error messages for detailed diagnostics.

Supporting additional waveform types.

This protocol ensures structured communication between the PC and STM32 for precise control of pulse generation.
