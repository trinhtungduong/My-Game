using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    public RigBuilder rigBuilder;
    public List<MultiAimConstraint> listAims;

    private void Start()
    {
        foreach (var component in listAims)
        {
            var data = component.data.sourceObjects;
            data.SetTransform(0, InputController.Instance.aimLookAt);
            component.data.sourceObjects = data;
        }
        rigBuilder.Build();
    }
}
