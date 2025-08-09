# Monogame Farming Simulator with Event-Driven Sentinel Monitoring

[![.NET Core](https://img.shields.io/badge/.NET-Core-blueviolet)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-Event%20Hub-blue)](https://azure.microsoft.com/services/event-hubs/)
[![Terraform](https://img.shields.io/badge/Terraform-Infrastructure-623CE4)](https://www.terraform.io/)
[![MonoGame](https://img.shields.io/badge/MonoGame-Simulation-red)](https://monogame.net/)
[![SQL](https://img.shields.io/badge/SQL-Data%20Storage-007fff)](https://monogame.net/)
[![Durable Functions](https://img.shields.io/badge/Durable%20Functions-Serverless%20Compute-green)](https://monogame.net/)

Welcome to the Monogame Farming Simulator project! This is an event-driven application that simulates a farm where "sentinels" (sensors) are 
placed to monitor environmental conditions like soil quality, moisture levels, and temperature. The simulator is built using Monogame for the 
game-like interface, and it integrates with a backend event-driven architecture using .NET Core, Azure Durable Functions, SQL Server with 
Change Data Capture (CDC), and SignalR for real-time updates.

# Project Overview
This project demonstrates an event-driven architecture in a fun, simulated farming environment:
- **Simulator (Monogame App)**: A 2D game using tilesets where you place sentinels on a farm map. Sentinels take periodic readings (soil, moisture, temperature) and can be "turned off" when worms collide with them.
- **Data Capture**: Readings and status changes (on/off) are saved to a SQL Server database. CDC is enabled on the tables to track changes automatically.
- **Event Processing**: A producer app queries CDC changes and publishes events to an Azure Durable Function orchestrator.
- **Orchestration and API Integration**: The orchestrator processes events, triggers activities to send data via HTTP to a .NET Core API, which broadcasts updates using SignalR to connected clients (e.g., a web dashboard).
- **User Interaction and Completion**: Users review data on the dashboard, confirm analysis (via a checkbox), which raises an external event to complete the orchestration. This triggers the simulator to turn sentinels back on.

The entire Azure infrastructure is built using Terraform, which provisions resources like Azure Function Apps, Storage Accounts (for Durable Functions state), SQL Databases, and Event Hubs.  Why event-driven? In real-world systems like IoT farming sensors, events (e.g., sensor readings) happen asynchronously. This architecture decouples components: the simulator doesn't wait for the dashboard, and vice versa. It uses
pub-sub patterns (producer as publisher, orchestrator as subscriber/orchestrator) for scalability—if you add more sentinels, the system handles increased events without redesign.

Currently, the focus is on sentinel status changes (on/off). Next steps will expand to react to readings like low moisture by adding visual indicators in the simulator.

# Architecture
Here's a high-level overview of the event-driven flow:

- **Monogame Simulator (.NET Core App)**: Publishes initial readings and handles collisions. Why Monogame? It's a lightweight framework for cross-platform 2D games, perfect for simulating dynamic farm elements like moving worms.
- **SQL Server Database**: Stores sentinel data in tables (e.g., SoilReadings, MoistureReadings, TemperatureReadings, SentinelStatus). CDC captures inserts/updates/deletes as events.
- **Producer App (.NET Core Console App)**: Queries CDC tables periodically and sends events to the orchestrator. This acts as the "publisher" in pub-sub.
- **Azure Durable Function (Orchestrator)**: Receives events, fans out to activity functions (e.g., one per reading type), and sends HTTP requests to the .NET Core API. Why Durable Functions? They handle stateful workflows in serverless Azure, ensuring orchestration persists even if activities fail or timeout—ideal for reliable event processing.
- **.NET Core API**: Receives HTTP payloads, processes them, and uses SignalR to broadcast real-time updates to clients. Why SignalR? It enables push notifications over WebSockets, keeping the UI live without polling.
- **Web Client (Dashboard)**: Displays data in DataTables.js. User confirms analysis, triggering an HTTP call back to the orchestrator to raise an external event and complete the workflow.
- **Feedback Loop**: Orchestrator completion signals the simulator to react (e.g., turn sentinels on).
- **GitHub Actions (CI/CD)**: This project uses GitHub Actions for Continuous Integration/Continuous Deployment (CI/CD), defined in .github/workflows/deploy.yaml.

# How It Works
**Event flow**:
1. Simulator saves changes; CDC captures.
2. Producer queries, publishes to orchestrator.
3. Orchestrator fans out (e.g., CallActivityAsync).
4. Activities POST to API.
5. API broadcasts via SignalR.
6. Dashboard confirms, raises event.
7. Orchestrator completes, signals simulator.

Teaches pub-sub: Decoupled, reliable with Durable Functions.

# Screenshots

**Farming Simulation in MonoGame**
The Monogame farm with sentinels (black spikes), worms (brown creatures), and other elements. Rain and clouds are animated indicating dynamic environment; worms turn off sentinels on collision.

<img width="1275" height="749" alt="monogame-simulator" src="https://github.com/user-attachments/assets/8bf5d4ce-565b-4ee8-86e5-bc91c1a0e470" />

**Azure Durable Function (Orchestrator)**
Console detailing HTTP requests to/from the orchestrator, including orchestration starts and activity executions. This traces the durable workflow for transparency.

<img width="997" height="373" alt="durable-functions" src="https://github.com/user-attachments/assets/e17375a1-180a-4910-8750-e411f70e8e07" />

**Web Client (Dashboard)**
Web dashboard showing sentinel data table with ID, saved date, status (on/off), moisture, and temperature. Green checkboxes allow confirming analysis, closing the loop.

<img width="1476" height="946" alt="web-app" src="https://github.com/user-attachments/assets/8263e66b-791b-4da5-910d-466444b326f6" />

