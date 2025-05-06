
# 🚦 Safety Pilot System


**AI-Driven Driving Simulation Platform for Road Safety Education**

The Safety Pilot System is a cross-platform, gamified learning environment designed to modernize driver education through AI-powered feedback, real-time simulation, and immersive training scenarios. Built using Unity, .NET Core, and Azure/OpenAI services, the system supports learners in mastering driving theory and practice within safe, controlled, and adaptive virtual settings.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Deployment](#deployment)
- [AI Integration](#ai-integration)
- [Modules & Scenes](#modules--scenes)
- [Performance Benchmarks](#performance-benchmarks)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Overview

Conventional driver education often lacks interactivity, personalization, and engagement. The Safety Pilot System addresses these challenges by offering a modular, data-driven learning tool that simulates real traffic environments and delivers real-time feedback through an intelligent AI assistant.

Designed for use in academic institutions, public safety campaigns, and training centers, the system is built for cross-device deployment including desktops, Android devices, and VR platforms.

---

## Key Features

- **Realistic Driving Simulation**: Powered by Unity with physics-based vehicle control and traffic systems
- **AI Feedback Engine**: Context-aware analysis of driving behavior using OpenAI/Gemini models
- **Gamified Learning**: Achievements, performance scoring, and adaptive difficulty levels
- **Cross-Platform Compatibility**: Runs on Windows, Linux, Android, and Oculus Quest 2
- **Modular Scene Library**: Includes scenarios for intersections, roundabouts, rainy weather, and more
- **Instructor Dashboard**: Assign modules, track progress, and download learner reports
- **Offline Support**: Key features accessible without an internet connection

---

## Technology Stack

| Layer             | Technology                    |
|------------------|-------------------------------|
| Simulation Engine| Unity (2021.3 LTS)             |
| Backend API      | ASP.NET Core / .NET 6+         |
| AI Services      | Azure OpenAI / Google Gemini   |


## Installation

### Download

Get the latest version of the Safety Pilot System for Windows:

👉 **[Download Safety Pilot for Windows (.zip)](https://safetypilotsystem.com/downloads/SafetyPilot_Windows_v1.0.zip)**

After downloading:
- Extract the ZIP file
- Run `SafetyPilot.exe`
- Follow the on-screen setup to start learning

> If SmartScreen or antivirus flags the file, choose “Run Anyway” (the app is safe and unsigned for now).


### Android (APK)


## Usage

* Run the application and log in or create a user profile
* Select a simulation module from the dashboard
* Complete the driving challenge and receive real-time AI feedback
* Review performance metrics and retry modules for improvement

---


## Deployment

* **Cloud Backend**: Deploy using Docker or Azure App Services
* **Mobile Builds**: Generate APKs using Unity's Android build support
* **Web Dashboard**: Deploy via Firebase Hosting or Vercel

---

## AI Integration

The system interfaces with third-party AI models for contextual feedback. It parses user actions in real time and submits structured queries to the AI service, receiving natural language guidance in return.

Example:

```json
{
  "scenario": "urban_left_turn",
  "user_behavior": {
    "signal_used": false,
    "speed": 45,
    "collision": true
  },
  "query_type": "feedback_request"
}
```

---

## Modules & Scenes

| Module Name         | Description                              |
| ------------------- | ---------------------------------------- |
| Basic Controls      | Steering, braking, acceleration          |
| Night Driving       | Low visibility, headlight usage          |
| Rainy Weather       | Low grip handling, braking on wet roads  |
| Roundabouts         | Signal timing, merging, and yielding     |
| Parking & Reversing | Spatial awareness and slow-speed control |

---

## Performance Benchmarks

| Spec Tier                 | FPS (Avg) | Load Time | RAM Usage | AI Latency |
| ------------------------- | --------- | --------- | --------- | ---------- |
| Minimum (i5/8GB)          | \~30 FPS  | 8 sec     | 2.8 GB    | 3–5 sec    |
| Recommended (i7/16GB GPU) | 60+ FPS   | 3 sec     | 2.0 GB    | <2 sec     |



## Contributing

We welcome pull requests and collaborations. To contribute:

1. Fork the repo and clone it locally
2. Create a feature branch
3. Commit and push changes
4. Open a pull request with clear descriptions

> All contributions must adhere to our coding standards and pass pre-merge testing workflows.


## License

This project is licensed under the MIT License. See [`LICENSE.md`](LICENSE.md) for details.

---

## Contact

For bug reports, partnership inquiries, or academic collaborations:

* **Email**: [support@safetypilotsystem.com](mailto:support@safetypilotsystem.com)
* **Website**: [www.safetypilotsystem.com](https://www.safetypilotsystem.com)
* **Maintainer**: Group BSE25-20, School of Computing and Informatics Technology, Makerere University

---

*Transforming road safety education through intelligent, immersive, and accessible technology.*



## Controls Summary

| Action          | Desktop Key | Mobile Button |
| --------------- | ----------- | ------------- |
| Accelerate      | `W / ↑`     | Pedal Icon    |
| Brake / Reverse | `S / ↓`     | Pedal Icon    |
| Steer           | `A / D`     | Arrows        |
| Gear Toggle     | `G`         | Gear Button   |
| Headlights      | `Spacebar`  | Bulb Icon     |
| AI Assist       | `P`         | Chat Icon     |



