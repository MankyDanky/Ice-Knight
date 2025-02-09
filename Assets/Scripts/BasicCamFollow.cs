using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

class CamShake
{
    private float bornTime;
    private readonly float strength;
    private readonly float magnitude;
    private readonly float ttl;
    private readonly float direction;
    private readonly float fade;

    public CamShake(float direction, float magnitude, float strength, float fade, float ttl)
    {
        bornTime = Time.time;
        this.strength = strength;
        this.fade = fade;
        this.magnitude = magnitude;
        this.ttl = ttl;
        this.direction = direction;
    }

    public Vector3 GetOffset()
    {
        float t = Time.time - bornTime;
        double r = magnitude / (t * fade + 0.74) * Math.Cos(t * strength + 0.74);
        return new Vector3((float)(Math.Cos(direction) * r), (float)(Math.Sin(direction) * r), 0);
    }

    public bool IsAlive()
    {
        return Time.time - bornTime <= ttl;
    }
}

public class BasicCamFollow : MonoBehaviour
{
    [SerializeField] public Camera cam;
    [SerializeField] public Transform[] targetsList = new Transform[0];
    [SerializeField] public float speed = 2.5f;
    [SerializeField] private List<CamShake> shakes = new();
    private Vector3 pos = Vector3.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }

        if (cam == null)
        {
            Debug.LogWarning("Camera not found! Please attach this script to a camera!");
        }

        pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetsList == null || targetsList.Length == 0) return;
        float newX = pos.x;
        float newY = pos.y;

        float targetX = 0;
        float targetY = 0;

        var count = 0;
        if (targetsList != null)
        {
            foreach (var target in targetsList)
            {
                if (!target.IsDestroyed())
                {
                    targetX = target.position.x;
                    targetY = target.position.y;
                    count++;
                }
            }
        }


        if (count == 0)
        {
            targetX = transform.position.x;
            targetY = transform.position.y;
        }
        else
        {
            targetX /= count;
            targetY /= count;
        }

        newX += (targetX - newX) * speed * Time.deltaTime;
        newY += (targetY - newY) * speed * Time.deltaTime;

        Vector3 offset = Vector3.zero;

        LinkedList<CamShake> removed = new LinkedList<CamShake>();
        foreach (CamShake shake in shakes)
        {
            if (shake.IsAlive())
            {
                offset += shake.GetOffset();
            }
            else
            {
                removed.AddLast(shake);
            }
        }

        foreach (var rm in removed)
        {
            shakes.Remove(rm);
        }

        pos = new Vector3(newX, newY, pos.z) + offset;
        transform.position = pos + offset;
    }

    public void ApplyShake(float direction, float magnitude = 1.0f, float strength = 1.0f, float fade = 1.0f,
        float ttl = 1.0f)
    {
        shakes.Add(new CamShake(direction, magnitude * 0.025f, strength * 80.0f, (1.0f / fade) * 50.0f, ttl));
    }
}