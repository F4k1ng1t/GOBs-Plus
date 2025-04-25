using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rotmaxxer : MonoBehaviour
{
    Goal[] mGoals;
    Action[] mActions;
    Action mChangeOverTime;
    const float TICK_LENGTH = 5.0f;
    public List<GameObject> playerLocations = new List<GameObject>();
    int currentActionIndex;

    public TextMeshProUGUI stats;
    public TextMeshProUGUI thoughts;
    // Start is called before the first frame update
    void Start()
    {
        // my inital motives/goals
        mGoals = new Goal[3];
        mGoals[0] = new Goal("Grades", 5);
        mGoals[1] = new Goal("Sleep", 6);
        mGoals[2] = new Goal("Rot", 6);

        mActions = new Action[5];

        mActions[0] = new Action("doom scroll");
        mActions[0].targetGoals.Add(new Goal("Grades", 0f));
        mActions[0].targetGoals.Add(new Goal("Sleep", -1f));
        mActions[0].targetGoals.Add(new Goal("Rot", +2f));

        mActions[1] = new Action("Academic comeback");
        mActions[1].targetGoals.Add(new Goal("Grades", -3f));
        mActions[1].targetGoals.Add(new Goal("Sleep", +1f));
        mActions[1].targetGoals.Add(new Goal("Rot", +1f));

        mActions[2] = new Action("sleep on the bed");
        mActions[2].targetGoals.Add(new Goal("Grades", 0f));
        mActions[2].targetGoals.Add(new Goal("Sleep", -5f));
        mActions[2].targetGoals.Add(new Goal("Rot", +1f));

        mActions[3] = new Action("eat unhealthy snacks");
        mActions[3].targetGoals.Add(new Goal("Grades", 0f));
        mActions[3].targetGoals.Add(new Goal("Sleep", +1f));
        mActions[3].targetGoals.Add(new Goal("Rot", -2f));

        mActions[4] = new Action("drink an energy drink");
        mActions[4].targetGoals.Add(new Goal("Grades", -2f));
        mActions[4].targetGoals.Add(new Goal("Sleep", +2f));
        mActions[4].targetGoals.Add(new Goal("Rot", -1f));

        // the rate my goals change just as a result of time passing
        mChangeOverTime = new Action("tick");
        mChangeOverTime.targetGoals.Add(new Goal("Grades", +1.5f));
        mChangeOverTime.targetGoals.Add(new Goal("Sleep", +0.5f));
        mChangeOverTime.targetGoals.Add(new Goal("Rot", +1f));

        Debug.Log("Starting clock. One hour will pass every " + TICK_LENGTH + " seconds.");
        InvokeRepeating("Tick", 0f, TICK_LENGTH);

        Debug.Log("Hit E to do something.");
    }

    void Tick()
    {
        // apply change over time
        foreach (Goal goal in mGoals)
        {
            goal.value += mChangeOverTime.GetGoalChange(goal);
            //Debug.Log(mChangeOverTime.GetGoalChange(goal));
            goal.value = Mathf.Max(goal.value, 0);
        }
        UpdateStats();
        // print results
         PrintGoals();
    }
   void UpdateStats()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        stats.text = goalString;
    }
    void UpdateThoughts()
    {
        string thought = "";
        switch (currentActionIndex)
        {
            case 0:
                thought = "time to doomscroll...";
                break;
            case 1:
                thought = "Gotta lock in... Academic comeback time...";
                break;
            case 2:
                thought = "sleepytime!";
                break;
            case 3:
                thought = "snacktime!";
                break;
            case 4:
                thought = "gotta stay awake... I'll drink an energy drink...";
                break;
            default:
                break;
        }
        thoughts.text = thought;
    }
    void PrintGoals()
    {
        string goalString = "";
        foreach (Goal goal in mGoals)
        {
            goalString += goal.name + ": " + goal.value + "; ";
        }
        goalString += "Discontentment: " + CurrentDiscontentment();
        Debug.Log(goalString);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("-- INITIAL GOALS --");
            //PrintGoals();

            Action bestThingToDo = ChooseAction(mActions, mGoals);
            DoAction(mActions[currentActionIndex]);
            UpdateThoughts();

            //Debug.Log("-- BEST ACTION --");
            Debug.Log("I think I will " + bestThingToDo.name);

            // do the thing
            foreach (Goal goal in mGoals)
            {
                goal.value += bestThingToDo.GetGoalChange(goal);
                goal.value = Mathf.Max(goal.value, 0);
            }

            //Debug.Log("-- NEW GOALS --");
            PrintGoals();
            UpdateStats();
        }
    }
    void DoAction(Action action)
    {
        Debug.Log(action);
        for (int i = 0; i <= 4; i++)
        {
            if (action == mActions[i])
            {
                playerLocations[i].SetActive(true);
                
            }
            else
            {
                playerLocations[i].SetActive(false);
            }
        }
    }
    Action ChooseAction(Action[] actions, Goal[] goals)
    {
        // ----- simple selection ---------
        // -- find the most pressing goal
        // -- choose the action with the greatest impact on that goal
        // --------------------------------

        //// find the most valuable goal to try and fulfill
        //Goal topGoal = goals[0];
        //foreach (Goal goal in goals)
        //{
        //    if (goal.value > topGoal.value)
        //    {
        //        topGoal = goal;
        //    }
        //}
        //Debug.Log("My most pressing need is to " + topGoal.name);

        //// find the best action to take
        //Action bestAction = actions[0];
        //float bestUtility = -actions[0].GetGoalChange(topGoal);

        //foreach (Action action in actions)
        //{
        //    // we invert the change because a low change value is good (we
        //    // want to reduce the value for the goal) but utilities are
        //    // typically scaled so high values are good.
        //    float utility = -action.GetGoalChange(topGoal);

        //    // we look for the lowest change (highest utility)
        //    if (utility > bestUtility)
        //    {
        //        bestUtility = utility;
        //        bestAction = action;
        //    }
        //}
        //// return the best action to be carried out
        //return bestAction;

        // ----- utitiliy ---------
        // -- find the action leading to the 
        // -- lowest discontentment
        // --------------------------------

        // find the action leading to the lowest discontentment
        Action bestAction = null;
        float bestValue = float.PositiveInfinity;

        //foreach (Action action in actions)
        //{
        //    float thisValue = Discontentment(action, goals);
        //    //Debug.Log("Maybe I should " + action.name + ". Resulting discontentment = " + thisValue);
        //    if (thisValue < bestValue)
        //    {
        //        bestValue = thisValue;
        //        bestAction = action;

        //    }
        //}

        // Change to a for loop to get the index
        for (int i = 0; i < actions.Length; i++)
        {
            float thisValue = Discontentment(actions[i], goals);
            //Debug.Log("Maybe I should " + action.name + ". Resulting discontentment = " + thisValue);
            if (thisValue < bestValue)
            {
                bestValue = thisValue;
                bestAction = actions[i];
                currentActionIndex = i;
            }
        }
        return bestAction;
    }

    float Discontentment(Action action, Goal[] goals)
    {
        // keep a running total
        float discontentment = 0f;

        // loop through each goal
        foreach (Goal goal in goals)
        {
            // calculate the new value after the action
            float newValue = goal.value + action.GetGoalChange(goal);
            newValue = Mathf.Max(newValue, 0);

            // get the discontentment of this value
            discontentment += goal.GetDiscontentment(newValue);
        }

        return discontentment;
    }

    float CurrentDiscontentment()
    {
        float total = 0f;
        foreach (Goal goal in mGoals)
        {
            total += (goal.value * goal.value);
        }
        return total;
    }
}
