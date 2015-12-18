using UnityEngine;
using System;
namespace TapRoot.Tendril
{
    public class TendrilTip : TendrilNode
    {
        public bool mainTip = false;

        internal float minimapVisWidth = 5f;

        public GameObject selectedMarker;
        public bool selected
        {
            set
            {
                if(selectedMarker != null)
                {
                    selectedMarker.SetActive(value);
                }
            }
        }

        // reference to the tendril base node
        [SerializeField]
        private TendrilRoot tendrilRoot;
        internal TendrilNode meshRoot;

        public TendrilTip tipPrefab;
        public GameObject nodePrefab;

        public BoxCollider2D tendrilCollider;
        internal Vector3    tc_localPosition; //offset of tendril collider from our position
        internal Quaternion tc_localRotation; //offset of tendril collider from our rotation

        // direction of growth
        private Vector2 direction;
        public Vector2 growDirection
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
                transform.rotation = Quaternion.LookRotation(Vector3.back, direction);
            }
        }

        //speed of the tip
        public float growthRate = 2.0f;

        protected override void Awake()
        {
            base.Awake();
            SetState(new Growing(this));
            selected = false;
        }
        protected override void Start()
        {
            base.Start();

            tc_localPosition = tendrilCollider.transform.localPosition;
            tc_localRotation = tendrilCollider.transform.localRotation;
            tendrilCollider.transform.SetParent(transform.parent);

            minimapVis.GetComponent<MeshRenderer>().material.color = PlayerUI.playerColors[(int)tendrilRoot.GetPlayerNumber()];

            if (hud != null)
            {
                hud.maxAngle = maxBranchAngle;
                hud.minAngle = minBranchAngle;
                hud.Show();
            }
        }

        protected override void Update()
        {
            base.Update();

            tendrilCollider.transform.position = transform.position + transform.TransformVector(tc_localPosition);
            tendrilCollider.transform.rotation = tc_localRotation * transform.rotation;
        }

        public override void CatchFire()
        {
            base.CatchFire();

            if (mAudioSource && AudioClipManager.instance.OnFireSound)
            {
                mAudioSource.PlayOneShot(AudioClipManager.instance.OnFireSound);
            }

            tendrilRoot.TipCaughtFire();
        }

        public event Action newBranchCreated;
        void CreateNewBranch(Vector2 newDirection)
        {
            if (state == typeof(Growing))
            {
                TendrilNode parentNode;
                if (Vector3.Distance(transform.position, parent.transform.position) > 1f)
                {
                    parentNode = CreateNewNode();
                }
                else
                {
                    parentNode = parent;
                    return;
                }

                // create new tip
                TendrilTip newtip = Instantiate(tipPrefab);
                newtip.growDirection = direction;
                newtip.transform.position = transform.position + 0.5f * newtip.transform.up;
                newtip.transform.SetParent(tendrilRoot.transform);

                // set new tip references
                newtip.tendrilRoot = tendrilRoot;
                newtip.meshRoot = parentNode;

                // setup tendril heirarchy
                newtip.SetParent(parentNode);
                parentNode.AddChild(newtip);

                // change us
                meshRoot = parentNode;
                growDirection = newDirection;
                transform.position += 0.5f * transform.up;

                // play sound
                mAudioSource.PlayOneShot(AudioClipManager.instance.SplitSound);

                if (newBranchCreated != null)
                {
                    newBranchCreated.Invoke();
                }
            }
        }

        public TendrilNode CreateNewNode()
        {
            if(tendrilRoot == parent)
            {
                tendrilRoot.minimapVis.SetActive(true);

                float vislen = 2 * Vector2.Distance(parent.transform.position, tendrilRoot.transform.position);
                tendrilRoot.minimapVis.transform.localScale = new Vector3(minimapVisWidth, vislen, 1);
                tendrilRoot.minimapVis.transform.localPosition = new Vector3(0, vislen / 2, 1);
            }

            // spawn new node object and setup position/rotation
            GameObject newNode = Instantiate(nodePrefab);
            newNode.transform.localScale *= 0.5f;
            newNode.transform.position = transform.position;
            newNode.transform.rotation = Quaternion.LookRotation(Vector3.back, parent.transform.position - transform.position);
            newNode.transform.SetParent(tendrilRoot.transform);

            //set collider
            float length = 2 * Vector2.Distance(parent.transform.position, newNode.transform.position);
            BoxCollider2D newCollider = newNode.GetComponent<BoxCollider2D>();
            newCollider.size = new Vector2(1, length);
            newCollider.offset = new Vector2(0, 0.5f * length);

            // setup node component
            TendrilNode newNodeComponent = newNode.GetComponent<TendrilNode>();
            newNodeComponent.AddChild(this);    //new node children set
            newNodeComponent.SetParent(parent); //new node parent set

            // set up new node minimap vis
            newNodeComponent.minimapVis.transform.localScale = new Vector3(minimapVisWidth, length, 1);
            newNodeComponent.minimapVis.transform.localPosition = new Vector3(0, length / 2, 0);
            newNodeComponent.minimapVis.GetComponent<MeshRenderer>().material.color = PlayerUI.playerColors[(int)tendrilRoot.GetPlayerNumber()];

            // update parent references
            if (parent != null)
            {
                parent.RemoveChild(this);
                parent.AddChild(newNodeComponent); //old parent child set
            }

            parent = newNodeComponent; //my parent set

            return newNodeComponent;
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionStay2D(collision);
        }
        void OnCollisionStay2D(Collision2D collision)
        {
            if (state == typeof(Growing))
            {
                switch (collision.gameObject.layer)
                {
                    case Layers.Fire:
                        if (typeof(OnFire) != mState.GetType())
                        {
                            CatchFire();
                            if(mainTip)
                            {
                                tendrilRoot.ShakePlayerCamera(4);
                            }
                        }
                        break;
                    case Layers.Resources:
                        SetState(new Leeching(this, collision.gameObject));

                        if (mainTip)
                        {
                            tendrilRoot.ShakePlayerCamera(1);
                        }
                        break;
                    case Layers.LevelEdge:
                    case Layers.Rock:
                        SetState(new Alive(this));

                        if (mainTip)
                        {
                            tendrilRoot.ShakePlayerCamera(2);
                        }
                        if (mAudioSource && AudioClipManager.instance.HitRockSound)
                        {
                            mAudioSource.PlayOneShot(AudioClipManager.instance.HitRockSound);
                        }

                        break;
                    //case Layers.Tip:
                    case Layers.Tendril:

                        Debug.Log("Collided with a tendril");

                        TendrilNode node = collision.gameObject.GetComponent<TendrilNode>();

                        if(node == null)
                        {
                            node = collision.gameObject.GetComponentInParent<TendrilNode>();
                        }

                        // ignore hits for 50ms after branching
                        if (node.GetType() == typeof(TendrilTip) &&
                            ((Growing)mState).timeSinceBranch < 0.05f &&
                            ((TendrilTip)node).tendrilRoot.GetPlayerNumber() == tendrilRoot.GetPlayerNumber())// && node != parent))
                        {
                            Debug.LogFormat("ignoring hit {0}", tendrilRoot.GetPlayerNumber());
                            break;
                        }

                        // catch on fire
                        if (node != null && node.IsOnFire())
                        {
                            CatchFire();
                            if (mainTip)
                            {
                                tendrilRoot.ShakePlayerCamera(4);
                            }
                            break;
                        }

                        if(node != null)
                        {
                            // TODO have it catch fire when the fire reaches the collision point
                            node.NodeCaughtFire += CatchFire;
                        }

                        SetState(new Alive(this));

                        // effects
                        if (mainTip)
                        {
                            tendrilRoot.ShakePlayerCamera(2);
                        }
                        if (mAudioSource && AudioClipManager.instance.HitTendrilSound)
                        {
                            mAudioSource.PlayOneShot(AudioClipManager.instance.HitTendrilSound);
                        }

                        break;
                }
            }
        }

        // input functions (called down into by parent)
        public override void AccelerateGrowth()
        {

        }

        public float minBranchAngle = 10;
        public float maxBranchAngle = 170;

        public BranchHUD hud;
        private Vector2 currentBranchAim;

        // input functions (called into by TendrilRoot)
        public void StartBranch()
        {
            if (state == typeof(Growing))
            {
                // show UI
                hud.Show();
            }
        }
        public void EndBranch()
        {
            if (state == typeof(Growing))
            {
                // hide UI
                hud.Hide();

                // branch off
                if (ValidateBranchDirection(currentBranchAim))
                {
                    CreateNewBranch(currentBranchAim.normalized);
                }
            }
        }
        public void BranchAim(Vector2 input)
        {
            currentBranchAim = input;
            hud.SetAngle(currentBranchAim);
        }
        public bool ValidateBranchDirection(Vector2 dir)
        {
            return currentBranchAim.magnitude > 0.2f && Vector2.Angle(transform.up, dir) < maxBranchAngle
                                                     && Vector2.Angle(transform.up, dir) > minBranchAngle;
        }
    }
}