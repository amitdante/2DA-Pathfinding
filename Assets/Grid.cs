﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	public Transform player;
	public LayerMask obstacles;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid ;
	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start(){
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);
		CreateGrid ();
	}

	void CreateGrid(){
		grid = new Node[gridSizeX, gridSizeY];
		Vector2 worldBottomLeft = (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;
		for(int x = 0; x<gridSizeX;x++){
			for (int y = 0; y < gridSizeY; y++) {
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius, obstacles));
				grid [x, y] = new Node (walkable, worldPoint, x, y);
			}
		}
	}

	public List<Node> GetNeighbors(Node node){
		List<Node> neighbors = new List<Node> ();  
		for(int x=-1; x<=1;x++){
			for(int y=-1; y<=1;y++){
				if (x == 0 && y == 0) {
					continue;
				}
					int checkX = node.gridX + x;
					int checkY = node.gridY + y;
					if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
						neighbors.Add (grid [checkX, checkY]);

				}
			}
		}
		return neighbors;
	}

	public Node NodeFromWorldPoint(Vector2 worldPosition){
		float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x, y];	
	}
	public List<Node> path;
	void OnDrawGizmos(){
		//Gizmos.DrawCube (transform.position, new Vector3 (gridWorldSize.x,1, gridWorldSize.y));
		if(grid!=null){
			foreach(Node n in grid){
				//Gizmos.color = (n.walkable) ? Color.white : Color.red;
				if(path!=null){
					if (path.Contains (n)) {
						Gizmos.color = Color.black;
						Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
					}
				}

			}
		}
	}
}
