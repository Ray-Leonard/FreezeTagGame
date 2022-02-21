using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeTagLogicControl : MonoBehaviour
{
    private List<AIAgentR3> agents = new List<AIAgentR3>();         // all the agents presented in scene
    private List<Renderer> agentRenderers = new List<Renderer>();   // all the agents' renderes to change materials.
    public AIAgentR3 chaser;   // keep a reference to the chaser
    private List<AIAgentR3> frozenAgents = new List<AIAgentR3>(); // contains all frozen agents 
    public Dictionary<AIAgentR3, AIAgentR3> saveDict = new Dictionary<AIAgentR3, AIAgentR3>();

    [SerializeField] float nontagFleeDistance = 4;
    [SerializeField] float chasedFleeDistance = 10;


    // a dictionary that sotres the materials of the characters
    // the key matches the NPC tag so one can easily change the NPC material using its tag
    private Dictionary<string, Material> materialDict = new Dictionary<string, Material>();
    [SerializeField] private Material[] materials;

    private void Awake()
    {
        // get a reference to all the AI agents in scene.
        foreach (Transform t in transform)
        {
            agents.Add(t.GetComponent<AIAgentR3>());
            agentRenderers.Add(t.GetComponentInChildren<Renderer>());
        }

        // set up the material dictionary here
        materialDict.Add("Freeze", materials[0]);
        materialDict.Add("Chaser", materials[1]);
        materialDict.Add("Nontag", materials[2]);
        materialDict.Add("Chased", materials[3]);
    }

    void Start()
    {
        // before starting the game, assign one Chaser randomly.
        int index = Random.Range(0, agents.Count);
        agents[index].gameObject.tag = "Chaser";
        chaser = agents[index];

        // assign a random target
        chaser.target = GetNextTarget();

        UpdateBehaviorByTag();
        UpdateMaterials();
    }

    void Update()
    {
        UpdateFrozenAgents();
        UpdateBehaviorByTag();
        UpdateMaterials();
        
    }

    // different tag is associate with different behaviors.
    private void UpdateBehaviorByTag()
    {
        // nontag - wander, but flee if chaser is around(nontag flee distance)
        // chaser - pursue
        // chased - flee from chaser
        // freeze - still
        foreach(AIAgentR3 agent in agents)
        {
            string tag_ = agent.gameObject.tag;
            if(tag_ == "Nontag")
            {   
                // chaser is around, flee chaser
                if(Vector3.Distance(agent.transform.position, chaser.transform.position) < nontagFleeDistance)
                {
                    agent.behavior = AIAgentR3.Behavior.Flee;
                    //agent.behavior = AIAgentR3.Behavior.Wander;

                    agent.target = chaser.transform;
                    // reset wander parameters to have realistic behavior
                    agent.ResetWanderParam();
                }
                // Freeze friend around, unfreeze friend.
                else if (Unfreeze(agent, out AIAgentR3 target))
                {
                    agent.behavior = AIAgentR3.Behavior.Pursue;
                    agent.target = target.transform;
                    // update save dict
                    saveDict[target] = agent;

                    // reset wander parameters to have realistic behavior
                    agent.ResetWanderParam();
                }
                else
                {
                    agent.behavior = AIAgentR3.Behavior.Wander;

                }
            }
            else if(tag_ == "Chaser")
            {
                // here we only modify the behavior, not the target of the chaser
                // the target of the chaser is handled in the AIAgentR3 collision resolution
                agent.behavior = AIAgentR3.Behavior.Pursue;
            }
            else if(tag_ == "Chased")
            {
                if (Vector3.Distance(agent.transform.position, chaser.transform.position) < chasedFleeDistance)
                {
                    agent.behavior = AIAgentR3.Behavior.Flee;
                    //agent.behavior = AIAgentR3.Behavior.Wander;

                    agent.target = chaser.transform;
                    // reset wander parameters to have realistic behavior
                    agent.ResetWanderParam();
                }
                else
                {
                    agent.behavior = AIAgentR3.Behavior.Wander;
                }
            }
            else if(tag_ == "Freeze")
            {
                agent.behavior = AIAgentR3.Behavior.Still;
            }
        }
    }

    // check if there's a frozen agent in the list to save
    // the one to be saved should not be saving by another nontag agent
    private bool Unfreeze(AIAgentR3 agent, out AIAgentR3 target)
    {
        target = null;
        foreach(AIAgentR3 a in frozenAgents)
        {
            saveDict.TryGetValue(a, out AIAgentR3 t);
            if(t == null || t == agent)
            {
                target = a;
                return true;
            }
            else
            {
                continue;
            }
        }

        
        return false;
    }

    // put all the frozen NPC into the hashset
    private void UpdateFrozenAgents()
    {
        // clear the set before adding
        frozenAgents.Clear();
        foreach(AIAgentR3 a in agents)
        {
            if(a.gameObject.tag == "Freeze")
            {
                frozenAgents.Add(a);
            }
        }
    }

    // get a random target from the nontag agents
    public Transform GetNextTarget()
    {
        List<Transform> possibleTargets = new List<Transform>();
        foreach(AIAgentR3 a in agents)
        {
            if(a.gameObject.tag == "Nontag")
            {
                possibleTargets.Add(a.transform);
            }
        }

        if(possibleTargets.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, possibleTargets.Count);
        // set the tag of the target
        possibleTargets[randomIndex].gameObject.tag = "Chased";

        // If this npc is selected as target, then has to remove it from the save dict if presented
        AIAgentR3 targetAgent = possibleTargets[randomIndex].gameObject.GetComponent<AIAgentR3>();
        if (saveDict.ContainsKey(targetAgent))
        {
            saveDict.Remove(targetAgent);
        }

        return possibleTargets[randomIndex];
    }

    // start next round of game
    public void NextGame(AIAgentR3 newChaser)
    {
        saveDict.Clear();

        // tag everyone to be "nontag", except the new chaser, we tag it "chaser". 
        foreach(AIAgentR3 a in agents)
        {
            if(a == newChaser)
            {
                a.gameObject.tag = "Chaser";
            }
            else
            {
                a.gameObject.tag = "Nontag";
            }
        }

        // then assign a new target for the chaser.
        chaser = newChaser;
        chaser.target = GetNextTarget();
    }

    private void UpdateMaterials()
    {
        // update the material of each NPC by their tag
        for(int i = 0; i < agents.Count; ++i)
        {
            agentRenderers[i].material = materialDict[agents[i].gameObject.tag];
        }
    }
}
