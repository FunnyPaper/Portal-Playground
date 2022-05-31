using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GlobalUtility
{
    namespace Logic
    {
        public enum LogicType
        {
            AND, OR, XOR, NAND, NOR, NXOR
        }

        public static class LogicFunctions
        {
            public static bool LogicAND(IEnumerable<bool> inputs)
                => inputs.All(i => i == true);

            public static bool LogicOR(IEnumerable<bool> inputs)
                => inputs.Any(i => i == true);

            public static bool LogicXOR(IEnumerable<bool> inputs)
                => inputs.Count(i => i == true) % 2 == 1;

            public static bool LogicNAND(IEnumerable<bool> inputs) 
                => !LogicAND(inputs);

            public static bool LogicNOR(IEnumerable<bool> inputs)
                => !LogicOR(inputs);

            public static bool LogicNXOR(IEnumerable<bool> inputs)
                => !LogicXOR(inputs);

            // Unity nie lubi switch expression ;___;
            public static bool EvaluateFunction(IEnumerable<bool> inputs, LogicType type)
            {
                bool res = false;
                switch(type)
                {
                    case LogicType.AND: res = LogicAND(inputs);
                        break;
                    case LogicType.OR: res = LogicOR(inputs);
                        break;
                    case LogicType.XOR: res = LogicXOR(inputs);
                        break;
                    case LogicType.NAND: res = LogicNAND(inputs);
                        break;
                    case LogicType.NOR: res = LogicNOR(inputs);
                        break;
                    case LogicType.NXOR: res = LogicNXOR(inputs);
                        break;
                    default: throw new ArgumentException("Cos poszlo nie tak");
                };
                return res;
            }
        }
    }

    namespace InheritBehaviour
    {
        [RequireComponent(typeof(AudioSource))]
        public abstract class Mechanism : MonoBehaviour
        {
            protected AudioSource _audioSource;

            [Header("Logic Functions")]
            public List<Signal> InputSignals = new List<Signal>();
            public Logic.LogicType LogicType = Logic.LogicType.AND;
            protected bool _check = false;

            // Start is called before the first frame update
            protected virtual void Start()
            {
                _audioSource = GetComponent<AudioSource>();
                foreach (Signal signal in InputSignals)
                {
                    signal.SignalReceivers.Add(this);
                }
            }

            // Update is called once per frame
            protected void Update()
            {
                //Check();
            }

            public abstract void Check();
        }

        public abstract class ContinousMechanism : Mechanism
        {
            [SerializeField] protected bool _activeRegardlessSignal = false;
        }

        [RequireComponent(typeof(Animator), typeof(AudioSource))]
        public abstract class Signal : MonoBehaviour
        {
            protected Animator _animator;
            protected AudioSource _audioSource;

            private bool _isActive = false;
            public bool IsActive
            {
                get => _isActive;
                set
                {
                    if (IsActive != value)
                    {
                        _isActive = value;
                        foreach (Mechanism receiver in SignalReceivers)
                        {
                            receiver.Check();
                        }
                    }
                }
            }

            [HideInInspector] public List<Mechanism> SignalReceivers = new List<Mechanism>();

            // Start is called before the first frame update
            protected virtual void Start()
            {
                _animator = GetComponent<Animator>();
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public abstract class Entity : MonoBehaviour
        {
            protected virtual void Update()
            {
                if (transform.position.y < -10)
                    Destroy();
            }

            public virtual void Destroy()
            {
                Destroy(this.gameObject);
            }
        }

        [RequireComponent(typeof(Rigidbody))]
        public abstract class AIEntity : Entity
        {
            [SerializeField]
            protected float _health = 100;
            public float Health => _health;

            public bool IsAlive => _health > 0;

            protected Rigidbody rBody;

            protected virtual void Start()
            {
                rBody = GetComponent<Rigidbody>();
            }
            
            protected virtual void Kill()
            {
                this.enabled = false;
            }


            public virtual void TakeDamage(float damagePoints)
            {
                if(IsAlive)
                {
                    _health -= damagePoints;
                    if (!IsAlive)
                        Kill();
                }
            }
        }
    }

    public interface Useable
    {
        void Use();
    }
}
