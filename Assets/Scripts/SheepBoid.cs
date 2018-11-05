using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implementation of http://www.csc.kth.se/utbildning/kth/kurser/DD143X/dkand13/Group9Petter/report/Martin.Barksten.David.Rydberg.report.pdf
/// </summary>
public class SheepBoid : MonoBehaviour {

    [Tooltip("Minimum distance to a predator at which a sheep starts panicking")]
    public float flightZoneRadius = 7;

    [Tooltip("Velocity under which the sheep do not move")]
    public float minVelocity = 0.1f;

    [Tooltip("Max velocity of the sheep")]
    public float maxVelocityBase = 1;

    [Tooltip("Max velocity of the sheep when a predator is close")]
    public float maxVelocityFear= 4;

    /// <summary>
    /// Vector created by applying all the herding rules
    /// </summary>
    Vector3 targetVelocity;

    /// <summary>
    /// Actual velocity of the sheep
    /// </summary>
    Vector3 velocity;

    [Header("Cohesion")]

    public float weightCohesionBase = 0.5f;
    public float weightCohesionFear = 5;

    [Header("Separation")]

    public float weightSeparationBase = 2;
    public float weightSeparationFear = 0;

    [Header("Alignment")]

    public float alignementZoneRadius = 3;

    public float weightAlignmentBase = 0.1f;
    public float weightAlignmentFear = 1;

    [Header("Escape")]

    public float weightEscape = 6;

    Transform predator;

    void Start()
    {
        predator = GameObject.FindGameObjectWithTag("Predator").transform;
    }

    /// <summary>
    /// Sigmoid function, used for impact of second multiplier
    /// </summary>
    /// <param name="x">Distance to the predator</param>
    /// <returns>Weight of the rule</returns>
    float P(float x)
    {
        //Change 20 to 0.3
        return (1 / Mathf.PI) * Mathf.Atan((flightZoneRadius - x) / 0.3f) + 0.5f;
    }

    /// <summary>
    /// Combine the two weights affecting the rules
    /// </summary>
    /// <param name="mult1">first multiplier</param>
    /// <param name="mult2">second multipler</param>
    /// <param name="x">distance to the predator</param>
    /// <returns>Combined weights</returns>
    float CombineWeight(float mult1,float mult2,float x)
    {
        return mult1* (1+P(x) * mult2);
    }

    /// <summary>
    /// In two of the rules, Separation and Escape, nearby objects are prioritized higher than
    ///those further away. This prioritization is described by an inverse square function
    /// </summary>
    /// <param name="x">Distance to the predator</param>
    /// <param name="s">Softness factor</param>
    /// <returns></returns>
    float Inv(float x, float s)
    {
        //Avoid dividing by zero using espilon
        float value = x / s + Mathf.Epsilon;

        //Do not use the pow function since it can be quite expensive
        return 1 / (value * value);
    }

    /// <summary>
    /// The Cohesion rule is calculated for each sheep s with position sp. The Cohesion vector
    ///coh(s) is directed towards the average position Sp.The rule vector is calculated
    ///with the function
    ///coh(s) = Sp − sp/|Sp − sp|
    /// </summary>
    /// <returns>coh(s) the cohesion vector</returns>
    Vector3 RuleCohesion()
    {
        //Find the average position
        Vector3 averagePosition = Vector3.zero;

        foreach(SheepBoid sheep in SheepHerd.Instance.sheeps)
        {
            averagePosition += sheep.transform.position;
        }

        averagePosition /= SheepHerd.Instance.sheeps.Length;

        return (averagePosition - transform.position) / (averagePosition - transform.position).magnitude;
    }

    /// <summary>
    /// The Separation rule is calculated for each sheep s with position sp. The contribution
    ///of each nearby sheep si
    ///is determined by the inverse square function of the distance
    ///between the sheep with a softness factor of 1. This function can be seen in Formula
    ///(3.3). The rule vector is directed away from the sheep and calculated with the
    ///function
    ///sep(s) = sum(n,i)(sp − sip/|sp − sip| * inv(|sp − sip|, 1))
    /// </summary>
    /// <returns>sep(s) the separation vector</returns>
    Vector3 RuleSeparation()
    {
        Vector3 separationVector = Vector3.zero;

        foreach (SheepBoid sheep in SheepHerd.Instance.sheeps)
        {
            if (sheep == this)
                continue;

            Vector3 sheepToSheep = transform.position - sheep.transform.position;
            float sheepToSheepMagnitude = sheepToSheep.magnitude;
            separationVector += (sheepToSheep / sheepToSheep.magnitude) * Inv(sheepToSheepMagnitude, 1);
        }

        return separationVector;
    }

    /// <summary>
    /// The Alignment rule is calculated for each sheep s. Each sheep si within a radius of
    ///50 pixels has a velocity siv that contributes equally to the final rule vector.The size
    ///of the rule vector is determined by the velocity of all nearby sheep N.The vector is
    ///directed in the average direction of the nearby sheep.The rule vector is calculated
    ///with the function
    ///ali(s) = sum(Siv,N)
    ///where
    ///N = {si: si ∈ S ∩ |sip − sp| ≤ 50}
    /// </summary>
    /// <returns>ali(s) the alignement vector</returns>
    Vector3 RuleAlignment()
    {
        Vector3 alignmentVector = Vector3.zero;

        foreach(SheepBoid sheep in SheepHerd.Instance.sheeps)
        {
            if (sheep == this)
                continue;

            if (Vector3.Distance(sheep.transform.position,transform.position)<= alignementZoneRadius)
            {
                alignmentVector += sheep.velocity;
            }
        }

        return alignmentVector/ (SheepHerd.Instance.sheeps.Length-1);
    }

    /// <summary>
    /// The Escape rule is calculated for each sheep s with a position sp. The size of the
    ///rule vector is determined by inverse square function(3.3) of the distance between
    ///the sheep and predator p with a softness factor of 10. The rule vector is directed
    ///away from the predator and is calculated with the function
    ///esc(s) = sp − pp / |sp − pp| * inv(|sp − pp|, 10)
    /// </summary>
    /// <returns>esc(s) the escape vector</returns>
    Vector3 RuleEscape()
    {
        Vector3 predatorToSheep = transform.position - predator.position;
        float predatorToSheepMagnitude = predatorToSheep.magnitude;
        //Change 10 to 2
        return (predatorToSheep / predatorToSheepMagnitude) * Inv(predatorToSheepMagnitude, 2);
    }

    /// <summary>
    /// Get the intended velocity of the sheep by applying all the herding rules
    /// </summary>
    /// <returns>The resulting vector of all the rules</returns>
    Vector3 ApplyRules()
    {
        float distanceToPredator = (transform.position - predator.position).magnitude;

        Vector3 v = Vector3.zero;
        v += RuleCohesion() * CombineWeight(weightCohesionBase, weightCohesionFear, distanceToPredator);
        v += RuleSeparation() * CombineWeight(weightSeparationBase, weightSeparationFear, distanceToPredator);
        v += RuleAlignment() * CombineWeight(weightAlignmentBase, weightAlignmentFear, distanceToPredator);
        v += RuleEscape() * weightEscape;
        v += Pen.Instance.RuleEnclosed(transform.position)*3;

        Debug.DrawRay(transform.position, RuleCohesion() * CombineWeight(weightCohesionBase, weightCohesionFear, distanceToPredator), Color.green);
        Debug.DrawRay(transform.position, RuleSeparation() * CombineWeight(weightSeparationBase, weightSeparationFear, distanceToPredator),Color.black);
        Debug.DrawRay(transform.position, RuleAlignment() * CombineWeight(weightAlignmentBase, weightAlignmentFear, distanceToPredator), Color.yellow);
        Debug.DrawRay(transform.position, RuleEscape() * weightEscape, Color.red);
        Debug.DrawRay(transform.position, Pen.Instance.RuleEnclosed(transform.position) *3, Color.cyan);

        return v;
    }

    void Update()
    {
        targetVelocity = ApplyRules();
    }

    /// <summary>
    /// Move the sheep based on the result of the rules
    /// </summary>
    void Move()
    {
        float distanceToPredator = (transform.position - predator.position).magnitude;

        //Clamp the velocity to a maximum that depends on the distance to the predator
        float currentMaxVelocity = Mathf.Lerp(maxVelocityBase, maxVelocityFear, 1-(distanceToPredator / flightZoneRadius));
        targetVelocity = Vector3.ClampMagnitude(targetVelocity, currentMaxVelocity);

        //Ignore the velocity if it's too small
        if (targetVelocity.magnitude < minVelocity)
            targetVelocity = Vector3.zero;

        //Draw the velocity as a blue line coming from the sheep in the scene view
        Debug.DrawRay(transform.position, targetVelocity, Color.blue);

        velocity = targetVelocity;

        //Make sure we don't move the sheep verticaly by misstake
        velocity.y = 0;

        //Move the sheep
        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    void LateUpdate()
    {
        Move();
     }
}
