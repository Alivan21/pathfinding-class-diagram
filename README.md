# Path Finding Class Diagram

A Windows Forms application that generates and visualizes class diagrams with path finding capabilities. This application helps in understanding and analyzing class relationships in software systems.

## Features

- Class diagram generation and visualization
- Path finding between classes using Dijkstra's algorithm
- Multiple diagram layout strategies
- Performance tracking for diagram operations
- File service for saving and loading diagrams
- Extractor service for processing class information

## Prerequisites

- Windows operating system
- .NET Framework 4.8
- Visual Studio 2019 or later (for development)

## Project Structure

```
PathFindingClassDiagram/
├── Models/                 # Data models for diagrams and relationships
├── Services/              # Core business logic and services
│   ├── DiagramLayouts/    # Different layout strategies
│   └── Interfaces/        # Service interfaces
├── UI/                    # User interface components
│   └── ViewModels/        # View models for UI
├── Pathfinding/           # Path finding algorithms
├── Helpers/               # Utility classes
└── Properties/            # Application properties and resources
```

## Key Components

- **DiagramGrid**: Manages the grid-based layout of class diagrams
- **DijkstraRouter**: Implements path finding between classes
- **DiagramService**: Handles diagram generation and manipulation
- **ExtractorService**: Processes and extracts class information
- **FileService**: Manages file operations for saving/loading diagrams

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Build the solution
4. Run the application

## Usage

1. Launch the application
2. Use the interface to:
   - Generate class diagrams
   - Analyze relationships between classes
   - Find paths between different classes
   - Save and load diagrams

## Development

The project follows a clean architecture pattern with:

- Clear separation of concerns
- Interface-based design
- Dependency injection
- MVVM pattern for UI components

## License

This project is licensed under the terms included in the LICENSE.txt file.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request
