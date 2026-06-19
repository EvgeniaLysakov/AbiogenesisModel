# Contributing to AbiogenesisModel

Thank you for contributing. This document describes how to work on the project, coding and review standards, and a short near-term implementation plan (2–3 day "near-sight" roadmap) to get the project back into active development.

## Values

- Keep changes small and reviewable.
- Follow the repository's .editorconfig formatting rules.
- Tests are required for bug fixes and new behavior.

## Getting started

1. Clone the repository and open in Visual Studio 2022.
2. Restore NuGet packages and build the solution.
3. Use the provided YAML configs in the `Config` folders to run sample sessions.

## Branching and Pull Requests

- Use feature branches prefixed with `feature/`, bug fixes with `fix/`.
- Target the `main` branch with pull requests.
- Keep PRs under ~400 lines where reasonable.
- Provide a short description, testing steps, and link related issue(s).

## Coding standards

- Follow `.editorconfig` in the repository. Indentation, naming and code style are enforced there.
- Prefer existing project patterns (Service attributes, factory classes, controller pattern, small immutable objects).
- Public APIs must be documented with XML comments.

## Tests

- Unit tests live in the `AbiogenesisModelTest` project.
- Run tests locally before submitting a PR. Use `dotnet test` or Test Explorer in Visual Studio.
- Add tests to cover regressions and new behavior.

## Local debugging tips

- The WPF application project is `AbiogenesisModel.Wpf` (MainWindow and viewmodels). Use the WPF app for interactive UI checks.
- Telemetry providers and statistics live in `AbiogenesisModel.Telemetry`.
- Domain model and controllers are in `AbiogenesisModel.Lib`.

## Near-term plan (2–3 days)

This roadmap is designed to be completed in 2–3 working days (approx. 16–24 hours). Tasks are ordered by priority; each task includes an estimated time and acceptance criteria.

1. Restore and complete UI data bindings and plot rendering (6–9 hours)
   - Tasks:
     - Finish `FillPlotData` implementation for `LabeledBarPlotItem` and ensure `LinePlotItem` / `BarPlotItem` rendering is correct.
     - Wire ViewModels to the WPF controls where missing (check `MainWindow.xaml` / `MainWindow.xaml.cs` and `MainVm`).
     - Ensure plots refresh correctly when data updates and axes autoscale.
   - Acceptance criteria:
     - Plots render without exceptions, labels and titles display, and live updates appear when telemetry changes.

2. Stabilize telemetry collection and UI feed (4–6 hours)
   - Tasks:
     - Review telemetry providers (Nucleotide, Molecule, Strand) for null-safety and performance.
     - Ensure `SimulationStatisticsHub` aggregates and exposes data in a WPF-friendly way (INotifyPropertyChanged / observable collections).
     - Fix scroll behavior and feed autoscroll edge cases.
   - Acceptance criteria:
     - Statistics updates propagate to UI, no UI thread freezes, feed autoscroll respects user scroll position.

3. Add/repair unit tests for core controllers and model invariants (4–6 hours)
   - Tasks:
     - Add tests for `SimulationWorldController`, `PondController`, `MoleculeController`, `StrandController`.
     - Verify constructors and validation logic (e.g., `Molecule` constructor argument messages and timestamp behavior).
   - Acceptance criteria:
     - Tests cover main invariants and pass in CI locally.

4. Small quality-of-life fixes and documentation (2–3 hours)
   - Tasks:
     - Improve exception messages where missing.
     - Implement `GetHashCode` for types that override `Equals` (if missing).
     - Update README or add short developer notes for running the app.
   - Acceptance criteria:
     - Clearer errors, consistent equality/hash implementation, and updated README/developer notes.

## How to pick the next task

- Start with UI/plot rendering; that gives immediate visible feedback and helps prioritize telemetry changes.
- If UI work stalls (e.g., missing data contract issues), switch to telemetry or tests.

## Communication

- Log work in the issue tracker (create small issues for each subtask). Reference them from PRs.
- Keep changes small and focused; prefer multiple small PRs over a single large change.

## Contact

- If unsure about a design decision, open an issue describing options and tradeoffs.

---

This CONTRIBUTING.md should serve both as a developer onboarding guide and a short plan for re-focusing the project over the next 2–3 days.