using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour {
	Grid grid;
	public Transform seeker;
	public Transform target;

	void Awake(){
		grid = GetComponent<Grid> ();
	}

	void Update(){
		FindPath (seeker.position, target.position);
	}

	void FindPath(Vector2 startPos , Vector2 targetPos){
		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		List<Node> openSet = new List<Node> ();
		HashSet<Node> closedSet = new HashSet<Node> ();

		openSet.Add (startNode);

		while(openSet.Count>0){
			Node currentNode = openSet [0];
			for(int i = 1;i<openSet.Count;i++){
				if(openSet[i].FCost<currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost<currentNode.hCost){
					currentNode = openSet [i];

				}

			}

			openSet.Remove (currentNode);
			closedSet.Add (currentNode);

			if(currentNode==targetNode){
				RetracePath (startNode, targetNode);
				return;
			}

			foreach(Node neighbor in grid.GetNeighbors(currentNode)){
				if (!neighbor.walkable || closedSet.Contains (neighbor)) {
					continue;
				}
				int newMovementCost = currentNode.gCost + GetDistance (currentNode, neighbor);
				if(newMovementCost < neighbor.gCost || !openSet.Contains(neighbor)){
					neighbor.gCost = newMovementCost;
					neighbor.hCost = GetDistance (neighbor, targetNode);
					neighbor.parent = currentNode;
					if(!openSet.Contains(neighbor)){
						openSet.Add (neighbor);
					}
				}
			}	


		}

	}
	int GetDistance(Node nodeA, Node nodeB){
		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if(distX>distY){
			return 14 * distY + 10 * (distX - distY);
		}
		return 14 * distX + 10 * (distY - distX);
	}

	void RetracePath(Node startNode, Node endNode){
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while(currentNode != startNode){
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse ();
		StartCoroutine (Move (path));
		grid.path = path;
	}
	IEnumerator Move(List<Node> path){
		int i = 0;
		if (i < path.Count) {
			seeker.position = Vector2.MoveTowards ((Vector2)seeker.position, path [i].worldPosition, 1 * Time.deltaTime);
		}else{
			StopCoroutine ("Move");
		}
		yield return new WaitForSeconds (0.5f);
		Move (path);
		
	}

}
