﻿using UnityEngine;
using System.Collections;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
	public enum CaseType
	{
		Solid,
		Hole,
		Wall,
		Crieur
	}

	public GameObject player;

	public GameObject prefabGround;
	public GameObject prefabWall;
	public GameObject prefabCrieur;
    
	public int seed = 0;
	public bool fixSeed = false;
	public int difficulty = 1;
	
	private CaseType[][] level;
	
	private const int nbLines = 3;
	private const int nbColumns = 14;
	
	private const int marginHole = 2;
	private const int marginWall = 4;
	private const int marginCrieur = 2;
	
	private const float probalityFixWall = 0.2f;
	private const float probalityMultiWall = 0.1f;
	private const float reduceProbalityHoleInit = 0.1f;
	private const float reduceProbalityHoleWhenWall = 0.6f;
	private const float reduceProbalityHoleMulti = 0.02f;

	public Vector2 sizeCase = new Vector2(1.2f, 5);
	
	// Use this for initialization
	void Start()
	{
		ProceduralProcess();

		float x, y;
		for(int i  = 0; i < nbLines; i++)
		{
			for(int j = 0; j < nbColumns; j++)
			{
				x = transform.position.x + ((float)j - nbColumns / 2 + 0.5f) * sizeCase.x;
				y = transform.position.y + ((float)i - nbLines / 2) * sizeCase.y;

				GameObject clone;

				switch (level[i][j])
				{
				case CaseType.Solid:

					clone = (GameObject)Instantiate(prefabGround, new Vector3(x, y, 0), Quaternion.identity);
					clone.transform.parent = transform;

					break;
				case CaseType.Wall:

					clone = (GameObject)Instantiate(prefabGround, new Vector3(x, y, 0), Quaternion.identity);
					clone.transform.parent = transform;
					
					y += sizeCase.y / 2;
					
					clone = (GameObject)Instantiate(prefabWall, new Vector3(x, y, 0), Quaternion.identity);
					clone.transform.parent = transform;

					break;
				case CaseType.Crieur:
				
					clone = (GameObject)Instantiate(prefabGround, new Vector3(x, y, 0), Quaternion.identity);
					clone.transform.parent = transform;
					
					y += sizeCase.y / 4;
					
					clone = (GameObject)Instantiate(prefabCrieur, new Vector3(x, y, 0), Quaternion.identity);
					clone.GetComponent<JukeBox>().player = player;
					clone.transform.parent = transform;
				
					break;
				}
			}
		}
	}
	
	void ProceduralProcess ()
	{
		if (fixSeed)
			Random.seed = seed;
		
		level = new CaseType[nbLines][];
		for (int i = 0; i < nbLines; i++)
			level[i] = new CaseType[nbColumns];
		
		PlaceWalls();
		PlaceHoles();
		PlaceCrieur();
	}
	
	void PlaceWalls()
	{
		for (int i = 0; i < nbLines; i++)
		{
			if (i > 0 && level[i-1].Contains(CaseType.Wall))
				continue;
			
			if (!RandomByDifficulty(probalityFixWall, probalityMultiWall))
				continue;
			
			int column = Random.Range(marginWall, nbColumns - marginWall);
			level[i][column] = CaseType.Wall;
		}
	}
	
	void PlaceHoles()
	{
		int column;
		for (int i = 0; i < nbLines; i++)
		{
			float reduceProbality = reduceProbalityHoleInit;
			if (level[i].Contains(CaseType.Wall))
			{
				int caseWall = 0;
				for(int j = 0; j < nbColumns; j++)
				{
					if (level[i][j] == CaseType.Wall)
					{
						caseWall = j;
						break;
					}
				}
				
				do { column = Random.Range(marginHole, caseWall);
				} while (!ConditionHole(i, column));
				level[i][column] = CaseType.Hole;
				
				do { column = Random.Range(caseWall + 1, nbColumns - marginHole);
				} while (!ConditionHole(i, column));
				level[i][column] = CaseType.Hole;
				
				reduceProbality = reduceProbalityHoleWhenWall;
			}
			else
			{
				do { column = Random.Range(marginHole, nbColumns - marginHole);
				} while (!ConditionHole(i, column));
				level[i][column] = CaseType.Hole;
			}
			
			while (RandomByDifficulty(1f - reduceProbality, reduceProbalityHoleMulti))
			{
				do { column = Random.Range(marginHole, nbColumns - marginHole);
				} while (!ConditionHole(i, column));
				level[i][column] = CaseType.Hole;
				
				reduceProbality = reduceProbality * 2 + 0.1f;
			}
		}
	}
	
	bool ConditionHole(int i, int j)
	{
		return level[i][j] == CaseType.Solid
			&& (i <= 0 || level[i-1][j] == CaseType.Solid)
				&& (i >= nbLines - 1 || level[i+1][j] == CaseType.Solid);
	}
	
	void PlaceCrieur()
	{
		int column;
		int numberCrieur = difficulty / 4 + 1;

		int n = 0;
		for (int i = 0; i < nbLines; i++)
		{
			if (n >= numberCrieur)
				break;

			if (nbLines - i > numberCrieur - n && !RandomByDifficulty((float)numberCrieur / 3.0f, 0f))
				continue;

			do { column = Random.Range(marginCrieur, nbColumns - marginCrieur);
			} while (level[i][column] != CaseType.Solid);
			level[i][column] = CaseType.Crieur;
			n++;
		}
	}
	
	bool RandomByDifficulty(float baseValue, float difficultyMultiplier)
	{
		return Random.value < baseValue + difficultyMultiplier * difficulty;
	}
}
