using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;

public class LSystemScript : MonoBehaviour
{
    [SerializeField] private int iterations = 4;
    [SerializeField] private float startSize = 1f;
    [SerializeField] private float length = 10f;
    [SerializeField] private float angle = 30f;
    [SerializeField] private GameObject branch;

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
        float currentSize = startSize * 0.04f;

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

            sb.Append('<');

            currentString = sb.ToString();
            sb.Length = 0;
        }
        resultingString = currentString;

        Debug.Log("Generate Tree: " + currentString);

        Vector3 initialPosition = transform.position;
        Vector3 branchPosition;
        Vector3 branchOrientation;

        foreach (char c in currentString)
        {
            switch (c)
            {
                case 'F':   //draw line
                    branchPosition = initialPosition;
                    branchPosition += Vector3.up * length;

                    GameObject treeSegment = Instantiate(branch);
                    treeSegment.transform.parent = this.transform;
                    //LineRenderer lineRenderer = treeSegment.GetComponent<LineRenderer>();
                    //lineRenderer.SetPosition(0, initialPosition);
                    //lineRenderer.SetPosition(1, transform.position);
                    //lineRenderer.startWidth = currentSize;
                    //lineRenderer.endWidth = currentSize;

                    Vector3 scaler = treeSegment.transform.localScale;
                    scaler.z = Vector3.Distance(initialPosition, branchPosition) / 2;
                    treeSegment.transform.localScale = scaler;

                    treeSegment.transform.position = branchPosition;        // place bond here
                    treeSegment.transform.LookAt(initialPosition);            // aim bond at atom
                    initialPosition = branchPosition;


            break;
                case 'X':   //generate more 'F's
                    break;
                case '+':   //rotate clockwise Up/Down
                    transform.Rotate(Vector3.back * angle);
                    break;
                case '-':   //rotate anti-clockwise Up/Down
                    transform.Rotate(Vector3.forward * angle);
                    break;
                case '*':   //rotate clockwise Left/Right
                    transform.Rotate(Vector3.right * angle);
                    break;
                case '/':   //rotate anti-clockwise Left/Right
                    transform.Rotate(Vector3.left * angle);
                    break;
                case '<':   //smaller
                    currentSize -= 0.01f;
                    break;
                case '[':   //save current transform info
                    transformStack.Push(new LSystemTransformInfo()
                    {
                        position = transform.position,
                        rotation = transform.rotation
                    }
                    );
                    break;
                case ']':   //return to previous transform info
                    LSystemTransformInfo tfInfo = transformStack.Pop();
                    transform.position = tfInfo.position;
                    transform.rotation = tfInfo.rotation;
                    break;
                default:
                    throw new InvalidOperationException("Invalid L-Tree Operation: Character \"" + c + "\"");
            }
        }
    }

}
