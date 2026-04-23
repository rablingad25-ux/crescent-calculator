# Hollow Crescent Moon Volume Calculator

A modern .NET 10 web application that computes the volume of a **Hollow Crescent Moon** built from **two spheres** — two circles revolved around the same axis. The outer circle (radius `R`) and the cutter circle (radius `r`, center offset by `d`) each generate a sphere; subtracting the cutter sphere from the outer sphere gives a true 3D crescent moon. Hollowing it removes a similar inner crescent to leave a uniform wall of thickness `t`.

The frontend renders the crescent in real-time 3D using Three.js, with a soft lavender-purple aesthetic and three viewing modes (solid, cutaway, two-circles).

## Features

- ASP.NET Core 10 minimal-API backend
- Pure C# math library using the **closed-form sphere–sphere intersection volume**
- Interactive 3D viewer that builds the crescent surface from two spheres
- Lavender-purple modern UI, no frontend frameworks
- Dockerfile for one-command deployment to Render, Fly, Railway, etc.

## Project Layout

```
HollowCrescentMoonCalculator/
├── Program.cs
├── VolumeCalculator.cs
├── HollowCrescentMoonCalculator.csproj
├── Dockerfile
├── README.md
├── TermPaper.md
└── wwwroot/
    └── index.html
```

## Run Locally

Requires the **.NET 10 SDK**.

```bash
dotnet run
```

Then open <http://localhost:8080>.

## Run with Docker

```bash
docker build -t crescent-moon .
docker run -p 8080:8080 crescent-moon
```

## Deploy to Render

1. Push this folder to a GitHub repository.
2. On Render, create a new **Web Service** → **Deploy from a Git repository**.
3. Choose **Docker** as the runtime (Render auto-detects the `Dockerfile`).
4. Leave the port blank — Render injects `PORT` and the app reads it.

## API

`POST /api/calculate`

```json
{
  "outerRadius": 5,
  "cutterRadius": 4,
  "centerOffset": 3,
  "wallThickness": 0.4
}
```

Returns the outer sphere volume, overlap (lens) volume, crescent volume, inner cavity volume, and the hollow shell volume.

## The Math

Each generating circle, when revolved around the shared axis, produces a sphere. The crescent is the outer sphere with the cutter sphere subtracted. The volume of the lens-shaped intersection of two spheres of radii `R` and `r` whose centers are separated by `d` has a classical closed form:

```
V_intersection = π (R + r − d)² [d² + 2d(R + r) − 3(R − r)²] / (12 d)
```

valid when `|R − r| < d < R + r`. Then:

```
V_crescent = (4/3)π R³ − V_intersection
V_shell    = V_crescent(R, r, d) − V_crescent((R−t), (R−t)/R · r, (R−t)/R · d)
```

See `TermPaper.md` for the full derivation.
