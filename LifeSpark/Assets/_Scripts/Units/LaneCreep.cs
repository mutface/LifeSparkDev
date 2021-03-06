﻿using UnityEngine;
using System.Collections;

public class LaneCreep : UnitObject {

    public enum CreepState {
        Idle,
        Moving,
        Attacking,
        Attacked,
        Dead,
        Default
    }

    public float maxSpeed = 5.0f;
	public string playerName;
    public Transform target;
    public CreepManager creepManager;

    private CreepStateBase creepState;
    private CreepStateIdle creepStateIdle;
    private CreepStateMove creepStateMove;

	private GameObject sparkPointGroup;


    public int owner;

	// Use this for initialization
	void Awake () {
        creepStateIdle = new CreepStateIdle(this);
        creepStateMove = new CreepStateMove(this);

		sparkPointGroup = GameObject.Find("SparkPoints");
        creepState = creepStateIdle;
	}
	
	// Update is called once per frame
	void Update () {
        if (creepState != null)
            creepState.OnUpdate();
	}

    private void SwitchState(CreepState toState) {
        if (creepState.State == toState)
            return;

        if (creepState != null)
            creepState.OnExit();

        switch (toState) {
            case CreepState.Idle:
                creepState = creepStateIdle;
                break;
            case CreepState.Moving:
                creepState = creepStateMove;
                break;
        }

        creepState.OnEnter();
    }

    abstract class CreepStateBase {
        protected CreepState state;
        protected LaneCreep laneCreep;
        protected float startTime;
        
        public CreepState State { get { return state; } }

        public abstract void OnEnter();
        public abstract void OnUpdate();
        public abstract void OnExit();

        public CreepStateBase() { startTime = Time.time; }
    }

    class CreepStateIdle : CreepStateBase {
        public CreepStateIdle(LaneCreep pLaneCreep) {
            startTime = Time.time;
            state = CreepState.Idle;
            laneCreep = pLaneCreep;
        }

        public override void OnEnter() {
            startTime = Time.time;
        }

        public override void OnUpdate() {
            if (Vector3.SqrMagnitude(laneCreep.target.position - laneCreep.transform.position) > 2.0) {
				laneCreep.SwitchState(CreepState.Moving);
			}
        }

        public override void OnExit() {

        }
    }

    class CreepStateMove : CreepStateBase {
        public CreepStateMove(LaneCreep pLaneCreep) {
            startTime = Time.time;
            state = CreepState.Moving;
            laneCreep = pLaneCreep;
        }

        public override void OnEnter() {
            startTime = Time.time;
        }

        public override void OnUpdate() {
            if (laneCreep.target != null) {
				if (Vector3.SqrMagnitude(laneCreep.target.position - laneCreep.transform.position) > 2.0) {
					laneCreep.SwitchState(CreepState.Moving);
				
                	Vector3 targetPos = laneCreep.target.position;
                	Vector3 direction = (targetPos - laneCreep.transform.position).normalized;

                	laneCreep.transform.rotation = Quaternion.LookRotation(direction); // maybe use a slerp to limit angular speed
					laneCreep.transform.position += direction * laneCreep.maxSpeed * Time.deltaTime;
				}
				else {
					if (Vector3.SqrMagnitude(laneCreep.target.position - laneCreep.transform.position) <= 2.0) {
						laneCreep.target.GetComponent<SparkPoint>().SetSparkPointCapture(laneCreep.playerName, laneCreep.owner, true);
						Destroy(laneCreep.gameObject);
					}
				}
			}
            else {
                // what if the target has been destroyed?
                laneCreep.SwitchState(CreepState.Idle);
            }
        }

        public override void OnExit() {

        }
    }
}
