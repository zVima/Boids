using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
    public readonly int gridSize;
    public readonly float cellSize;
    private Dictionary<Vector2Int, List<Boid>> grid;

    // Constructor for creating a new Grid
    public Grid(float cellSize) {
        this.cellSize = cellSize;
        this.gridSize = Mathf.CeilToInt(Camera.main.orthographicSize * 2 / cellSize);
        this.grid = new Dictionary<Vector2Int, List<Boid>>();
    }

    // Adding a Boid to a Cell
    public void AddBoid(Boid boid) {
        Vector2Int cell = GetGridCell(boid.transform.position);
        if (!grid.ContainsKey(cell)) {
            grid[cell] = new List<Boid>();
        }

        grid[cell].Add(boid);
    }

    // Removing a Boid from a Cell
    public void RemoveBoid(Boid boid) {
        Vector2Int cell = GetGridCell(boid.transform.position);
        if (grid.ContainsKey(cell)) {
            grid[cell].Remove(boid);

            if (grid[cell].Count == 0) {
                grid.Remove(cell);
            }
        }
    }

    // Checks for all Boids in the same Cell and returns a List with all the Boids within the Search Radius
    public List<Transform> GetNearbyBoids(Boid boid, float searchRadius) {
        List<Transform> nearbyBoids = new List<Transform>();
        Vector2Int cell = GetGridCell(boid.transform.position);

        for (int x = cell.x - (int) (searchRadius / cellSize); x <= cell.x + (int) (searchRadius / cellSize); x++) {
            for (int y = cell.y - (int) (searchRadius / cellSize); y <= cell.y + (int) (searchRadius / cellSize); y++) {

                Vector2Int key = new Vector2Int(x, y);
                
                if (grid.ContainsKey(key)) {
                    foreach (Boid nearbyBoid in grid[key]) {
                        
                        if (Vector2.Distance(boid.transform.position, nearbyBoid.transform.position) <= searchRadius) {
                            nearbyBoids.Add(nearbyBoid.transform);
                        }
                    }
                }
            }
        }
        return nearbyBoids;
    }

    // Getting the Cell in the Grid, from a given Position
    private Vector2Int GetGridCell(Vector2 position) {
        return new Vector2Int((int) (position.x / cellSize), (int) (position.y / cellSize));
    }
}
