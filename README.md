# Airport Terminal Security Control (Discrete-Event Simulation Engine)

[![C#](https://img.shields.io/badge/Language-C%23%2012-239120.svg)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![.NET 8](https://img.shields.io/badge/Framework-.NET%208.0-512BD4.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![UI](https://img.shields.io/badge/UI-WinForms%20(Windows)-0078D6.svg)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/)
[![Plotting](https://img.shields.io/badge/Plotting-ScottPlot%205.1-6B46C1.svg)](https://scottplot.net/)
[![IDE](https://img.shields.io/badge/IDE-Visual%20Studio-purple.svg)](https://visualstudio.microsoft.com/)
[![Course](https://img.shields.io/badge/Course-Discrete%20Simulation-orange.svg)](https://fri.uniza.sk)

High-performance C# / Windows Forms desktop application and custom **discrete-event simulation (DES) engine** built on top of a **Monte Carlo replication core**, featuring **custom random variate generators** (Exponential, Triangular, Uniform, and Empirical) and **online statistical collectors** (sample and time-weighted), designed to model passenger flows, luggage X-ray scanning, metal detector inspections, and maximum daily capacity at an airport security terminal.

This project was developed as a university semester project (**Semester Project S2**) for the **Discrete Simulation (Diskrétna simulácia)** course at the Faculty of Management Science and Informatics, University of Žilina (**FRI UNIZA**).

---

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Simulation Architecture & Random Generators](#simulation-architecture--random-generators)
- [Airport Security Terminal Model](#airport-security-terminal-model)
- [Engine Verification, Warm-Up & Experimental Results](#engine-verification-warm-up--experimental-results)
- [Installation & How to Run](#installation--how-to-run)
- [GUI Controls & Usage](#gui-controls--usage)
- [Documentation](#documentation)

---

## Overview

Airport management is negotiating with an airline over the future utilization of a newly reconstructed airport terminal. To guarantee contractual service-level parameters, it is necessary to determine the **maximum 24-hour passenger capacity** of the terminal's security checkpoint while strictly satisfying passenger comfort constraints.

Every departing passenger must pass through security screening, which operates across **two parallel inspection lanes** (`PassengersFlow1` and `PassengersFlow2`), each equipped with:
- An **input conveyor belt** (default capacity: `4` crates) where passengers place their carry-on luggage into crates,
- An **X-ray luggage scanner** and an **output conveyor belt** (default capacity: `5` crates) that blocks further scanning when full,
- A **metal detector (RTG)** with an operator who immediately performs an on-the-spot **additional personal inspection** for `19%` of passengers (blocking the metal detector during the pat-down).

The simulation engine is implemented from scratch using only standard `.NET` libraries (`System.Random` and `PriorityQueue<TElement, TPriority>`), featuring:
1. **Universal Discrete-Event Simulation Core (`BaseEventSimulation`):** Extends the template-method `BaseMonteCarlo` engine with a priority-based event calendar (`PriorityQueue<EventCore, double>`), next-event time progression, interactive real-time playback (`ESleep`), speed scaling, pause/resume, and full-speed Turbo execution.
2. **Custom Probability Distribution Suite (`Generators`):** Inverse-transform and multi-interval implementations of Exponential, Triangular, Continuous/Discrete Uniform, and Continuous/Discrete Empirical distributions with deterministic hierarchical seeding and Chi-Square (`χ²`) goodness-of-fit validation.
3. **Sample & Time-Weighted Statistical Collectors (`SimpleStatistics` & `TimeStatistics`):** Custom classes computing sample means, time-weighted queue length integrals, sample standard deviations, and `90%` / `95%` confidence intervals without storing raw observations in memory.

---

## Key Features

- **Event-Driven Simulation Core Over Monte Carlo Replications (`BaseEventSimulation`):**
  - Combines discrete-event calendar scheduling (`PlanEvent`) in `DoReplication()` with multi-replication Monte Carlo orchestration (`BeforeSimulation`, `BeforeReplication`, `DoReplication`, `AfterReplication`, `AfterSimulation`).
  - Supports **real-time visual monitoring** via periodic `ESleep` system events (`_simTimeStep = 0.363 s`) with 11 adjustable speed multipliers (`0.05×` to `50×`), interactive **Pause / Resume**, and zero-delay **Turbo Mode**.
- **Analytical M/G/1 Queue Verification (`TestSimulation`):**
  - Includes a standalone single-server queueing model (`TestEvenSim`) verified against exact **Pollaczek–Khinchine** and **Little's Law** analytical values across `1,000` replications (`100,000` served customers per replication).
- **Dynamic Warm-Up Detection (`EWarmUp`):**
  - Automatically eliminates initial transient bias (empty terminal at `t = 0`) by tracking a **100-passenger moving average** (`WarmUpSamplesCount = 100`) of time spent in the system.
  - Once the relative change between consecutive moving averages stays below **`0.5%`** (`WarpUpTolerance = 0.005`) for **`50` consecutive departing passengers** (`WarpUpStabilityCount = 50`), all within-replication statistics (`SimpleStatistics` and `TimeStatistics`) are reset at the exact warm-up timestamp (`ResetStatistics(ExecuteTime)`).
- **Two-Stage Statistical Evaluation with 90% Confidence Intervals:**
  - Within each replication, `SimpleStatistics` tracks mean passenger time in the system and `TimeStatistics` integrates exact time-weighted queue lengths across both security lanes.
  - Across replications, final `SimpleStatistics` instances aggregate replication means and compute `90%` (`z = 1.645`) and `95%` (`z = 1.96`) confidence intervals.
- **Automated Capacity Sensitivity Analysis & Live Plotting (`ScottPlot.WinForms`):**
  - Dedicated **`Grafy a Štatistiky`** mode automatically evaluates `10` evenly spaced terminal capacities across a `±10%` range (`[0.9 · C, 1.1 · C]`) around the target capacity and plots the resulting comfort metrics in 3 interactive `ScottPlot` charts.
- **Strict Core / GUI Separation (`ISimGuiDelegate`):**
  - Complete decoupling between the simulation library (`DS_SEM_2`) and the Windows Forms presentation layer (`GUI`) using the observer/delegate pattern (`ISimGuiDelegate`) and double-buffered `DataGridView` tables.

---

## Simulation Architecture & Random Generators

### Class Hierarchy

```
BaseUniform<T> (where T : INumber<T>)
 ├── DiscreteUniform          (int: [min, max] inclusive)
 └── ContinuousUniform        (double: [min, max) half-open)

BaseInterval<T> (where T : INumber<T>)
 ├── IntervalDiscrete         (int sub-interval generator)
 └── IntervalContinuous       (double sub-interval generator)

BaseEmpiric<T, TInterval>
 ├── DiscreteEmpiric          (manages List<IntervalDiscrete>)
 └── ContinuousEmpiric        (manages List<IntervalContinuous>)

ExponentialDist               (Exponential generator via inverse transform: -ln(r) / λ)
TriangularDist                (Triangular generator via piecewise inverse CDF)

EventCore                     (Abstract base event with ExecuteTime & Execute())
 └── BaseEvent<TSimulation>
      ├── ESleep              (System event pacing real-time GUI updates & speed scaling)
      ├── EWarmUp             (Dynamic steady-state detector resetting replication statistics)
      ├── PassengerEvent      (Base class for passenger-lifecycle events)
      │    ├── EPassengerArrival
      │    ├── EStartLuggageDelivery
      │    ├── EEndLuggageDelivery
      │    ├── EStartRTG
      │    ├── EEndRTG
      │    ├── EStartPersonalScan
      │    ├── EEndPersonalScan
      │    └── EPassengerLeave
      ├── LuggageEvent        (Base class for crate/luggage scanner events)
      │    ├── EStartLuggageScan
      │    └── EEndLuggageScan
      └── TestEvent           (M/G/1 verification events: CustomerArrival, ServiceStart, ServiceEnd)

BaseMonteCarlo
 └── BaseEventSimulation      (PriorityQueue<EventCore, double> event calendar & execution loop)
      ├── AirportSim          (24-hour two-lane airport security checkpoint simulation)
      └── TestSimulation      (M/G/1 queueing benchmark for core validation)

Statistics
 ├── SimpleStatistics         (Unweighted mean, variance, standard error, 90% & 95% CI)
 └── TimeStatistics           (Time-weighted continuous integral ∫ L(t) dt / (T - T_0))
```

### Implemented Probability Distributions

| Generator Class | Distribution Type | Sampling Formula | Simulation Usage |
| :--- | :--- | :--- | :--- |
| **`ExponentialDist`** | Continuous Exponential | `X = -ln(r) / λ`, where `r ∈ (0, 1)` and `λ = C / 86400 s⁻¹` | **Passenger inter-arrival times** (Poisson arrival stream for 24h capacity `C`) |
| **`DiscreteEmpiric`** | Discrete Empirical | Interval `i` chosen if `C_{i-1} ≤ r₁ < C_i`; value `X = ⌊r₂ · (max_i - min_i + 1)⌋ + min_i` | **Crate count per passenger** (`0` crates: `15%`, `1` crate: `68%`, `2` crates: `17%`) |
| **`ContinuousUniform`** | Continuous Uniform | `X = r · (max - min) + min`, where `X ∈ [min, max)` | **Metal detector walk-through** (`[6, 27) s`) & **X-ray crate scan** (`[9, 46) s`) |
| **`TriangularDist`** | Continuous Triangular | `X = a + √(u(b-a)(c-a))` if `u < (c-a)/(b-a)`, else `b - √((1-u)(b-a)(b-c))` | **Additional personal pat-down** (`min = 10 s`, `max = 120 s`, `mode = 35 s`) |
| **`Random`** | Bernoulli / Discrete `50:50` | `r < p` via `System.Random.NextDouble()` | **Lane tie-breaking** (`p = 0.50`) & **Personal inspection trigger** (`p = 0.19`) |

---

## Airport Security Terminal Model

### 1. Stochastic Process Parameters

All operational durations and branching probabilities follow the assignment specification:

| Process / Operation | Distribution / Model | Parameters | Mean Value |
| :--- | :--- | :--- | :---: |
| **Passenger Arrivals** | Poisson Stream (`ExponentialDist`) | `λ = Capacity / 86,400 s⁻¹` (`24 hours = 86,400 s`) | `86,400 / Capacity s` |
| **Crate Count per Passenger** | Discrete Empirical (`DiscreteEmpiric`) | `0` crates (`0.15`), `1` crate (`0.68`), `2` crates (`0.17`) | `1.02 crates` |
| **Input Conveyor Capacity** | Configurable Parameter (`MaxLuggageBefore`) | Default: **`4` crates** per lane | — |
| **X-Ray Crate Inspection** | Continuous Uniform (`ContinuousUniform`) | `k ∈ [9, 46) s` | `27.50 s / crate` |
| **Output Conveyor Capacity** | Configurable Parameter (`MaxLuggageAfter`) | Default: **`5` crates** per lane (blocks scanner when full) | — |
| **Metal Detector Passage** | Continuous Uniform (`ContinuousUniform`) | `p ∈ [6, 27) s` | `16.50 s` |
| **Personal Inspection Rate** | Bernoulli Trial (`PersonalScanRand`) | `19%` (`p = 0.19`) of passengers after metal detector | — |
| **Personal Inspection Duration** | Continuous Triangular (`TriangularDist`) | `min = 10 s`, `max = 120 s`, `mode = 35 s` | `55.00 s` |

### 2. Two-Lane Queuing & Blocking Rules (`PassengersFlow`)

1. **Lane Selection (`EPassengerArrival`):**
   - Upon arrival, the passenger inspects the input luggage queues (`PassengersLuggageQueue`) of both lanes (`PassengersFlow1` and `PassengersFlow2`) and joins the shorter queue. If both queues have the same length, the lane is chosen at random (`50% / 50%`).
2. **Crate Placement & FIFO Discipline (`EStartLuggageDelivery` / `EEndLuggageDelivery`):**
   - When a passenger reaches the head of `PassengersLuggageQueue`, they place as many of their crates onto the input conveyor (`LuggagesBeforeQueue`) as available slots allow (`< MaxLuggageBefore`, default `4`).
   - Once all of a passenger's crates are placed on the input belt, the passenger leaves `PassengersLuggageQueue` and joins the FIFO queue for the metal detector (`PassengersRTGQueue`). Passengers with `0` crates still wait behind earlier passengers in `PassengersLuggageQueue` before advancing to `PassengersRTGQueue`.
3. **Metal Detector & On-the-Spot Personal Inspection (`EStartRTG` / `EEndRTG` / `EStartPersonalScan` / `EEndPersonalScan`):**
   - One passenger passes through the metal detector (`RTG`) at a time.
   - Immediately after passing through the detector, `19%` of passengers undergo an additional personal inspection (`PersonalScan`) performed by the operator right at the detector — **blocking subsequent passengers from entering the metal detector** until the pat-down finishes.
   - Passengers with `0` crates leave the security checkpoint immediately upon completing the metal detector (or personal inspection). Passengers with crates join `PassengersWaitingLuggageQueue`.
4. **X-Ray Scanning & Output Conveyor Blocking (`EStartLuggageScan` / `EEndLuggageScan` / `EPassengerLeave`):**
   - The X-ray scanner processes crates one by one from `LuggagesBeforeQueue` and places inspected crates onto `LuggagesAfterQueue` (capacity `MaxLuggageAfter`, default `5`).
   - If `LuggagesAfterQueue` is full (`5` crates), **the X-ray scanner cannot start scanning the next crate** (which remains on `LuggagesBeforeQueue`) until a passenger removes a crate from the output belt.
   - Once the passenger at the head of `PassengersWaitingLuggageQueue` collects all of their scanned crates from `LuggagesAfterQueue`, they leave the system (`EPassengerLeave`), freeing space on the output belt and potentially unblocking the X-ray scanner.

---

## Engine Verification, Warm-Up & Experimental Results

### 1. Random Variate Generator Validation (`χ²` Goodness-of-Fit Tests)

Both newly added continuous generators (`ExponentialDist` and `TriangularDist`) were visually inspected via histograms (`10,000` samples) and statistically validated using **Pearson's Chi-Square (`χ²`) Goodness-of-Fit Test** at significance level `α = 0.05` (`200` samples):

| Generator | Test Parameters | Bins (`k`) | Degrees of Freedom (`df`) | Test Statistic (`χ²_calc`) | Critical Value (`χ²_0.05`) | Result |
| :--- | :--- | :---: | :---: | :---: | :---: | :---: |
| **`ExponentialDist`** | `λ = 0.1` | `8` | `7` | **`6.116`** | `14.067` | **Pass** (`χ²_calc < χ²_0.05`, `H₀` not rejected) |
| **`TriangularDist`** | `min = 0, max = 100, mode = 50` | `10` | `9` | **`5.834`** | `16.919` | **Pass** (`χ²_calc < χ²_0.05`, `H₀` not rejected) |

---

### 2. Discrete-Event Core Verification (Analytical M/G/1 Queue Benchmark)

To verify the event calendar (`PriorityQueue`), time progression, `SimpleStatistics`, and `TimeStatistics`, a single-server **M/G/1 queueing system** (`TestSimulation`) was simulated for **`1,000` replications** (`100,000` served customers per replication) with:
- **Customer Arrivals:** `ExponentialDist` with `λ = 0.025 s⁻¹` (mean inter-arrival time `E[A] = 1 / λ = 40 s`)
- **Service Time:** `TriangularDist` with `a = 12 s, b = 48 s, c = 30 s` (mean `E[S] = 30 s`, variance `Var(S) = 54 s²`, second moment `E[S²] = 954 s²`, server utilization `ρ = λ · E[S] = 0.75`)

By the **Pollaczek–Khinchine formula** and **Little's Law**, the exact analytical steady-state values are compared against the simulation below:

| Performance Metric | Analytical Formula | Exact Value | Simulated Mean (`1,000` reps) | 90% Confidence Interval |
| :--- | :---: | :---: | :---: | :---: |
| **Mean Waiting Time in Queue (`W_q`)** | `W_q = (λ · E[S²]) / (2 · (1 - ρ))` | **`47.7000 s`** | **`47.6255 s`** | `[47.5773 ; 47.6737] s` |
| **Mean Queue Length (`L_q`)** | `L_q = λ · W_q` | **`1.1925 customers`** | **`1.1906 customers`** | `[1.1893 ; 1.1920] customers` |

---

### 3. Dynamic Warm-Up Phase (`EWarmUp`)

Because the terminal starts empty at the beginning of each replication, early passengers experience artificially short waiting times. The `EWarmUp` event dynamically detects when the simulation reaches steady state:
1. **Sliding Window:** Stores the system residence times of the last **`100` departing passengers** (`WarmUpSamples`).
2. **Relative Deviation Check:** Compares the current 100-passenger moving average against the previous passenger's moving average; requires a relative difference below **`0.5%`** (`WarpUpTolerance = 0.005`).
3. **Stability Confirmation:** The relative deviation must remain within `0.5%` continuously for **`50` consecutive departing passengers** (`WarpUpStabilityCount = 50`). If any sample exceeds the threshold, the stability counter resets to `0`. Once achieved, `ResetStatistics(ExecuteTime)` clears all cumulative statistics and starts steady-state measurement.

---

### 4. Terminal Capacity Optimization (24-Hour Horizon)

**Goal:** Determine the maximum number of passengers (`Capacity`) that can pass through the security checkpoint in **24 hours (`86,400 s`)** while guaranteeing all three **passenger comfort constraints** with **90% statistical confidence**:
1. **Mean time spent in the security system:** `≤ 10 minutes` (`600 s`)
2. **Mean number of passengers waiting to pick up scanned luggage:** `≤ 10 passengers`
3. **Mean number of passengers waiting to start inspection (luggage drop-off queue):** `≤ 20 passengers`

After narrowing down the critical load interval to **`5,710 – 5,730` passengers / 24 hours**, batch experiments of **`10,000` replications per capacity** were executed (`Program.cs` → `SimulationResults.csv`). The binding bottleneck is the **input queue of passengers waiting to hand over their crates** (`AverageLuggageQueueLengthFinal`):

| 24h Capacity | Mean Time in System (`s`) | 90% CI — Time in System (`s`) | Mean Luggage Pickup Queue | 90% CI — Pickup Queue | Mean Entry Queue (Bottleneck) | 90% CI — Entry Queue (Limit `≤ 20`) | Comfort Met? |
| :-: | :---: | :---: | :---: | :---: | :---: | :---: | :-: |
| **`5,717`** | **`452.99 s`** (`07:32.99`) | `[449.29170 ; 456.69445]` | **`2.370`** | `[2.36612 ; 2.37462]` | **`19.668`** | **`[19.42233 ; 19.91338]`** | **YES (Optimal)** |
| `5,718` | `> 453 s` | `< 600 s` | `≈ 2.37` | `< 10.0` | `19.946` | `[19.69685 ; 20.19428]` | **NO** (Upper CI `> 20`) |

> **Conclusion:** At `5,718` passengers per 24 hours, the upper bound of the 90% confidence interval for the entry queue reaches `20.19428`, exceeding the maximum comfort limit of `20` waiting passengers. Therefore, the **maximum guaranteed 24-hour capacity of the airport terminal is `5,717` passengers**.

---

## Installation & How to Run

### Prerequisites

- **Operating System:** Windows 10 / 11
- **SDK:** [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (`net8.0` / `net8.0-windows`)
- **IDE:** [Visual Studio 2022 / 2026](https://visualstudio.microsoft.com/) (with *.NET desktop development* workload)

### Steps

1. **Clone the repository:**
   ```bash
   git clone https://github.com/lubomirbojda/Discrete-Event-Simulation-Engine
   cd Discrete-Event-Simulation-Engine
   ```

2. **Open the solution:**
   - Open `DS_SEM_2.slnx` in Visual Studio.

3. **Choose the Startup Project:**
   - **Airport Simulation Monitor (WinForms GUI):** Right-click `GUI` in Solution Explorer → **Set as Startup Project**.
   - **Batch Capacity Evaluator & CSV Exporter (Console):** Right-click `DS_SEM_2` in Solution Explorer → **Set as Startup Project**.

4. **Build & Run:**
   - Select `Release` or `Debug` configuration and press `F5` or `Ctrl + F5`.
   - Or run directly from the CLI:
     ```bash
     # Launch the main Windows Forms Airport Simulation Monitor
     dotnet run --project GUI -c Release

     # Run the console batch capacity experiment (exports SimulationResults.csv)
     dotnet run --project DS_SEM_2 -c Release
     ```

---

## GUI Controls & Usage

### Top Control Bar
- **`Spustiť simuláciu` / `Zastaviť simuláciu`:** Starts or stops the simulation asynchronously on a background worker thread.
- **`Pozastaviť` / `Pokračovať`:** Pauses and resumes event execution mid-replication.
- **`Rýchlosť (- / +)`:** Adjusts real-time simulation playback speed across 11 presets (`0.05×`, `0.1×`, `0.25×`, `0.5×`, `0.75×`, `1×`, `2.5×`, `5×`, `10×`, `25×`, `50×`).
- **`Turbo`:** Disables `ESleep` delays and live table rendering to execute replications at maximum CPU speed.
- **Model Parameters:**
  - **`Replikácie`** (default `1,000`): Number of Monte Carlo replications.
  - **`Kapacita`** (default `5,717`): Target 24-hour passenger arrival count.
  - **`Max pred sken`** (default `4`): Maximum crate capacity of the input conveyor belt before the X-ray scanner.
  - **`Max po sken`** (default `5`): Maximum crate capacity of the output conveyor belt after the X-ray scanner.
  - **`Vlastný Seed`**: Optional fixed integer seed for deterministic reproducibility.

### 1. Tab `Sledovanie simulácie` (Live Checkpoint Monitor)
- Displays real-time state tables (`DataGridView`) and active operator statuses for both security lanes (**Flow 1** and **Flow 2**):
  - **`(1/2) Odovzdávanie`:** Passengers waiting in the input queue (`ID Pasažier`, `Batožiny`).
  - **`(1/2) Pred RTG`:** Passengers who placed their crates and are waiting for the metal detector, plus live labels for **`Pasažier RTG`** and **`Osobná prehliadka`**.
  - **`(1/2) Pred skenerom`:** Crates currently on the input conveyor belt (`ID Batožina`, `Vlastník`), plus the live **`Skenuje sa`** indicator.
  - **`(1/2) Po skeneri`:** Inspected crates waiting on the output conveyor belt (`ID Batožina`, `Vlastník`).
  - **`(1/2) Posuvný pás`:** Passengers who completed screening and are waiting to collect their crates.
- **`ŠTATISTIKY` Panel:** Continuously updates the cumulative cross-replication mean and `90%` confidence interval for all three comfort metrics.

### 2. Tab `Grafy a Štatistiky` (Automated ±10% Capacity Sensitivity Analysis)
- Automatically enables **Turbo Mode** and evaluates `10` capacity points across `[0.9 × Kapacita, 1.1 × Kapacita]`, plotting live `ScottPlot` dependency curves for:
  1. **Priemerná doba bezpečnostnej kontroly** (Capacity vs. Mean Time in System in seconds)
  2. **Priemerný počet ľudí čakajúcich na prevzatie batožiny** (Capacity vs. Mean Luggage Pickup Queue Length)
  3. **Priemerný počet ľudí čakajúcich na začiatok prehliadky** (Capacity vs. Mean Entry Queue Length)

---

## Documentation

Complete technical documentation (in Slovak) is included in the repository root:

- **[Project Documentation (PDF)](https://github.com/lubomirbojda/Discrete-Event-Simulation-Engine/blob/main/Documentation.pdf)**
  - Histograms (`10,000` samples) and Pearson's `χ²` goodness-of-fit tests (`200` samples, `α = 0.05`) for the Exponential and Triangular generators
  - Object-oriented architecture of the universal discrete-event simulation core (`BaseEventSimulation`) and analytical validation on an **M/G/1 queueing model** (Pollaczek–Khinchine & Little's Law)
  - Complete **Event Graph (Udalostný diagram)** and transition descriptions for the airport security terminal
  - Dynamic warm-up detection algorithm (`EWarmUp`) and statistical evaluation of the `5,717` passenger/24h terminal capacity limit with sensitivity charts

> **Note:** PDF documentation is written in the **Slovak language**.

---

## Screenshots

<p align="center">
  <img width="1549" height="848" alt="Snímka obrazovky 2026-11-01 001935" src="https://github.com/user-attachments/assets/f15cbe79-0de9-47b9-a5d0-796fe51a5905" />
  <img width="1551" height="847" alt="Snímka obrazovky 2026-11-01 002033" src="https://github.com/user-attachments/assets/2e8210cd-c076-4ce6-bf17-9a2d2ee4ad6e" />

</p>
