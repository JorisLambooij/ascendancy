using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class LSystemScript : MonoBehaviour
{
    [SerializeField] private int iterations = 4;
    [SerializeField] private float length = 10f;
    [SerializeField] private float angle = 30f;
    [SerializeField] private GameObject stem;
    [SerializeField] private List<GameObject> branch;
    [SerializeField] private List<GameObject> leaves;

    [SerializeField] private string axiom = "X";
    [SerializeField] private List<LSystemRule> rulesInput;


    [SerializeField]
    private string resultingString;

    private Dictionary<char, string> rules;

    private Stack<LSystemTransformInfo> transformStack;
    private string currentString = string.Empty;
    //private Vector3 initialPosition = new Vector3();

    void Start()
    {
        transformStack = new Stack<LSystemTransformInfo>();
        rules = new Dictionary<char, string>();

        foreach (LSystemRule rule in rulesInput)
        {
            rules.Add(rule.character, rule.stringContent);
        }

        Generate();
    }

    private void Generate()
    {
        UnityEngine.Random.InitState((int)(World.Instance.GetComponent<HeightMapGenerator>().perlinOffset.x * World.Instance.GetComponent<HeightMapGenerator>().perlinOffset.y));
        //float currentSize = 0.04f;

        currentString = axiom;
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < iterations; i++)
        {
            foreach (char c in currentString)
            {
                if (rules.ContainsKey(c))
                {
                    sb.Append(rules[c]);
                }
                else
                {
                    sb.Append(c.ToString());
                }
            }

            currentString = sb.ToString();
            sb.Length = 0;
        }
        resultingString = currentString;

        Debug.Log("Generate Tree: " + currentString);

        //Vector3 initialPosition = transform.position;
        Vector3 branchPosition = transform.position;
        Quaternion branchOrientation = transform.rotation;

        Vector3 targetPosition;
        GameObject treeSegment;
        Vector3 scaler;
        float scaleFactor = 0.1f * iterations/2;

        foreach (char c in currentString)
        {
            if (scaleFactor > 0f)
                scaleFactor -= 0.0005f;

            int model = 0;

            switch (c)
            {
                case 'F':   //leaves
                    targetPosition = branchPosition + (branchOrientation * transform.up) * length;

                    model = UnityEngine.Random.Range(0, leaves.Count - 1);
                    treeSegment = Instantiate(leaves[model]);
                    treeSegment.transform.parent = this.transform;

                    scaler = treeSegment.transform.localScale;
                    float scale = Vector3.Distance(branchPosition, targetPosition) / 1.5f;
                    
                    scaler.y = scale;

                    scaler.x = Mathf.Clamp(scaleFactor, 0.05f, 0.1f);
                    scaler.z = Mathf.Clamp(scaleFactor, 0.05f, 0.1f);

                    treeSegment.transform.localScale = scaler;
                    treeSegment.transform.rotation = branchOrientation;

                    treeSegment.transform.position = branchPosition;        // place bond here
                    branchPosition = targetPosition;
                    break;
                case 'S':   //generate stem
                    targetPosition = branchPosition + (branchOrientation * transform.up) * length;

                    treeSegment = Instantiate(stem);
                    treeSegment.transform.parent = this.transform;

                    scaler = treeSegment.transform.localScale;
                    scaler.y = Vector3.Distance(branchPosition, targetPosition) / 1.5f;

                    scaler.x = scaleFactor;
                    scaler.z = scaleFactor;

                    treeSegment.transform.localScale = scaler;
                    treeSegment.transform.rotation = branchOrientation;

                    treeSegment.transform.position = branchPosition;        // place bond here
                    branchPosition = targetPosition;
                    break;
                case 'X':   //generate more 'F's
                    break;
                case 'G':   //generate branches
                    targetPosition = branchPosition + (branchOrientation * transform.up) * length;

                    model = UnityEngine.Random.Range(0, leaves.Count - 1);
                    treeSegment = Instantiate(branch[model]);
                    treeSegment.transform.parent = this.transform;

                    scaler = treeSegment.transform.localScale;
                    scaler.y = Vector3.Distance(branchPosition, targetPosition) / 1.5f;

                    scaler.x = Mathf.Clamp(scaleFactor, 0.05f, 1f);
                    scaler.z = Mathf.Clamp(scaleFactor, 0.05f, 1f);

                    treeSegment.transform.localScale = scaler;
                    treeSegment.transform.rotation = branchOrientation;

                    treeSegment.transform.position = branchPosition;        // place bond here
                    branchPosition = targetPosition;
                    break;
                case '+':   //rotate clockwise Up/Down
                    branchOrientation *= Quaternion.AngleAxis(angle, transform.right);
                    //transform.Rotate(Vector3.back * angle);
                    break;
                case '-':   //rotate anti-clockwise Up/Down
                    branchOrientation *= Quaternion.AngleAxis(-angle, transform.right);
                    break;
                case '*':   //rotate clockwise Left/Right
                    branchOrientation *= Quaternion.AngleAxis(angle, transform.forward);
                    break;
                case '/':   //rotate anti-clockwise Left/Right
                    branchOrientation *= Quaternion.AngleAxis(-angle, transform.forward);
                    break;
                case '[':   //save current transform info
                    transformStack.Push(new LSystemTransformInfo()
                    {
                        position = branchPosition,
                        rotation = branchOrientation
                    }
                    );
                    break;
                case ']':   //return to previous transform info
                    LSystemTransformInfo tfInfo = transformStack.Pop();
                    branchPosition = tfInfo.position;
                    branchOrientation = tfInfo.rotation;
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-Tree Operation: Character \"" + c + "\"");
            }
        }
    }

}
