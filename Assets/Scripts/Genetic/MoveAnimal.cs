﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAnimal : MonoBehaviour
{
    public EvolutionManager evolutionManager;

    //movement variables
    private float moveCounter = 0;
    public float moveSeed; //unique added later
    private bool onEdge = false;
    private float floorDistance = 2f;
    private float animalDistance = 1f;

    void Start()
    {
        evolutionManager = GameObject.Find("GameEngine").GetComponent<EvolutionManager>();
    }

    void FixedUpdate()
    {
        Ray floorRay = new Ray(transform.position + transform.forward / 2, (transform.forward - transform.up).normalized);
        RaycastHit floorHit;
        moveCounter ++;
        Debug.DrawRay(floorRay.origin, floorRay.direction, Color.blue);
        Ray animalRay = new Ray (transform.position, transform.forward);
        RaycastHit animalHit;
        Debug.DrawRay(animalRay.origin, animalRay.direction, Color.red);
                        
        if (moveCounter % (10f*moveSeed) == 0)
        {
            if(!onEdge)
                transform.eulerAngles += new Vector3(0f, Random.Range(0f, 360f), 0f);
            else
            {
                onEdge = false;
                transform.eulerAngles += new Vector3(0f, Random.Range(160f, 200f), 0f); //fix getting stuck in corner
            }
        }
        else if (moveCounter % moveSeed == 0)
        {
            if (Physics.Raycast(floorRay, out floorHit, floorDistance)) //if floor ray hits something
                {
                    DNA thisAnimalDNA = gameObject.GetComponent<AnimalCore>().thisAnimal;
                    // if (thisAnimalDNA.eggsInCarton > 0 &&
                    if(thisAnimalDNA.refractoryPeriod <= gameObject.GetComponent<AnimalCore>().refractoryPeriodCounter)
                    {
                        if (Physics.Raycast(animalRay, out animalHit, animalDistance))
                        {
                            Debug.Log(animalHit.collider.gameObject.tag);
                            //putting food here for now
                            if(animalHit.collider.gameObject.tag == "food")
                            {
                                Destroy(animalHit.collider.gameObject);
                                gameObject.GetComponent<AnimalCore>().belly++;
                            }
                            //main animal if
                            if(animalHit.collider.gameObject.tag == "animal" && gameObject.GetComponent<AnimalCore>().belly > 0) //just belly of initiator or both?
                            {
                                GameObject otherAnimal = animalHit.collider.gameObject;
                                DNA otherAnimalDNA = otherAnimal.GetComponent<AnimalCore>().thisAnimal;
                                float fertilityCheck = Random.Range(0f, 1f); //just arbitrary for now, can be manipulated by nature
                                // if (otherAnimalDNA.eggsInCarton > 0 && 
                                    // (thisAnimalDNA.fertility + otherAnimalDNA.fertility) >= evolutionManager.populationControlModifier && //this is new, their combined fertility must be over popControl threshold
                                if ((thisAnimalDNA.fertility + otherAnimalDNA.fertility) >= fertilityCheck &&
                                    otherAnimalDNA.refractoryPeriod <= otherAnimal.GetComponent<AnimalCore>().refractoryPeriodCounter) //if they have eggs left and are ready to go
                                    gameObject.GetComponent<AnimalCore>().Diddle(gameObject, otherAnimal);
                            }
                        }
                        else //no edge and no other animal, move
                            transform.position += (transform.forward / 2);
                    }
                    else //no edge and no eggs left, move
                        transform.position += (transform.forward / 2);
                }
            else //no hit detected
                onEdge = true;
        }
    }

}
