using System;
using UnityEngine;
using UnityEngine.AI;

namespace Sjur.Units
{
    /// <summary>
    /// Core unit component handling movement, combat, and state management
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class Unit : MonoBehaviour
    {
        [SerializeField] private UnitData data;
        [SerializeField] private int teamId;

        private NavMeshAgent navAgent;
        private UnitState currentState = UnitState.Idle;
        private float currentHealth;
        private Unit currentTarget;
        private float attackCooldown = 0f;
        private Transform targetDestination;

        public UnitData Data => data;
        public int TeamId => teamId;
        public float CurrentHealth => currentHealth;
        public UnitState CurrentState => currentState;
        public bool IsDead => currentHealth <= 0;

        public event Action<Unit> OnUnitDied;
        public event Action<Unit, Unit> OnUnitAttacked;
        public event Action<Unit> OnUnitReachedDestination;

        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();

            if (data != null)
            {
                currentHealth = data.maxHealth;
                SetupNavAgent();
            }
        }

        private void SetupNavAgent()
        {
            navAgent.speed = data.moveSpeed;
            navAgent.stoppingDistance = data.attackRange * 0.9f;
        }

        private void Update()
        {
            if (IsDead) return;

            attackCooldown -= Time.deltaTime;

            switch (currentState)
            {
                case UnitState.Moving:
                    UpdateMovement();
                    break;

                case UnitState.Attacking:
                    UpdateCombat();
                    break;

                case UnitState.Idle:
                    // Check for nearby enemies
                    SearchForEnemies();
                    break;
            }
        }

        private void UpdateMovement()
        {
            // Check if we reached destination
            if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    OnUnitReachedDestination?.Invoke(this);
                    SetState(UnitState.Idle);
                }
            }

            // Check for enemies in range while moving
            SearchForEnemies();
        }

        private void UpdateCombat()
        {
            // Check if target is still valid
            if (currentTarget == null || currentTarget.IsDead)
            {
                currentTarget = null;
                SetState(UnitState.Idle);
                return;
            }

            // Check if target is in range
            float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

            if (distance > data.attackRange)
            {
                // Move towards target
                navAgent.SetDestination(currentTarget.transform.position);
            }
            else
            {
                // Stop and attack
                navAgent.ResetPath();

                // Face target
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                // Attack if cooldown ready
                if (attackCooldown <= 0f)
                {
                    AttackTarget(currentTarget);
                }
            }
        }

        private void SearchForEnemies()
        {
            // Simple sphere check for nearby enemies
            Collider[] colliders = Physics.OverlapSphere(transform.position, data.attackRange);

            foreach (var col in colliders)
            {
                Unit enemy = col.GetComponent<Unit>();
                if (enemy != null && enemy.TeamId != teamId && !enemy.IsDead)
                {
                    SetTarget(enemy);
                    return;
                }
            }
        }

        private void AttackTarget(Unit target)
        {
            if (target == null || target.IsDead) return;

            float damage = data.CalculateDamage(target.Data);
            target.TakeDamage(damage, this);

            attackCooldown = 1f / data.attackSpeed;

            OnUnitAttacked?.Invoke(this, target);
        }

        public void TakeDamage(float damage, Unit attacker)
        {
            if (IsDead) return;

            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Die();
            }
            else if (currentState != UnitState.Attacking && attacker != null)
            {
                // Retaliate if not already in combat
                SetTarget(attacker);
            }
        }

        private void Die()
        {
            currentHealth = 0;
            SetState(UnitState.Dead);
            navAgent.enabled = false;

            OnUnitDied?.Invoke(this);

            // Disable or destroy after animation
            Destroy(gameObject, 2f);
        }

        public void MoveTo(Vector3 destination)
        {
            if (IsDead) return;

            targetDestination = null;
            navAgent.SetDestination(destination);
            SetState(UnitState.Moving);
        }

        public void MoveTo(Transform destination)
        {
            if (IsDead) return;

            targetDestination = destination;
            navAgent.SetDestination(destination.position);
            SetState(UnitState.Moving);
        }

        public void SetTarget(Unit target)
        {
            if (IsDead) return;

            currentTarget = target;
            SetState(UnitState.Attacking);
        }

        private void SetState(UnitState newState)
        {
            currentState = newState;
        }

        public void Initialize(UnitData unitData, int team)
        {
            data = unitData;
            teamId = team;
            currentHealth = data.maxHealth;
            SetupNavAgent();
        }

        public void SetTeam(int team)
        {
            teamId = team;
        }
    }

    /// <summary>
    /// Unit state machine states
    /// </summary>
    public enum UnitState
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }
}
