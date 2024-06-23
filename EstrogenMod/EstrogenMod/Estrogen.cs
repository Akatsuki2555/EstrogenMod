using System;
using System.Reflection;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace EstrogenMod
{
    public class Estrogen : Mod
    {
        public override string ID => "Estrogen";
        public override string Version => "1.0";
        public override string Author => "アカツキ";

        public override void ModSetup()
        {
            base.ModSetup();

            SetupFunction(Setup.OnLoad, Mod_Load);
            SetupFunction(Setup.Update, Mod_Update);
        }

        Keybind interact = new Keybind("estrogenInteract", "Interact", KeyCode.F);

        private void Mod_Update()
        {
            if (_camera == null) _camera = Camera.main;

            var player = GameObject.Find("PLAYER");
            foreach (var raycast in Physics.RaycastAll(_camera.transform.position, _camera.transform.forward, 3f))
            {
                if (raycast.collider.gameObject.name == "estrogen(Clone)")
                {
                    FsmVariables.GlobalVariables.GetFsmBool("GUIuse").Value = true;

                    if (interact.GetKeybindDown())
                    {
                        GameObject.Destroy(raycast.collider.gameObject);

                        MasterAudio.PlaySound("eating");
                    }
                }

                var e = raycast.collider.gameObject.GetComponent<EstrogenBox>();
                if (raycast.collider.gameObject.name == "estrogen box(Clone)" && e != null)
                {
                    FsmVariables.GlobalVariables.GetFsmBool("GUIuse").Value = true;
                    FsmVariables.GlobalVariables.GetFsmString("GUIinteraction").Value = $"TAKE ESTROGEN PILL ({e.pills} REMAINING)";

                    if (interact.GetKeybindDown())
                    {
                        e.pills--;
                        SpawnE(e.gameObject.transform.position);
                    }
                }

                
            }
        }

        private GameObject _prefab;
        private Camera _camera;

        public override void ModSettings()
        {
            base.ModSettings();

            var amount = Settings.AddSlider(this, "amt", "Amount", 1, 100, 1);
            Settings.AddButton(this, "Spawn Estrogen", () =>
            {
                for (int i = 0; i < amount.GetValue(); i++)
                {
                    SpawnE(GameObject.Find("PLAYER").transform.position);
                }
            });
            Settings.AddButton(this, "Spawn Estrogen Box", () =>
            {
                SpawnEBox();
            });
        }

        private void Mod_Load()
        {
            ModConsole.Print("Estrogen injected into your veins!");
            var bundle = LoadBundle();
            _prefab = bundle.LoadAsset<GameObject>("Estrogen");

            bundle.Unload(false);
        }

        private void SpawnE(Vector3 pos)
        {
            var instance = GameObject.Instantiate(_prefab);
            instance.transform.position = pos;
            instance.AddComponent<Rigidbody>();
            instance.MakePickable();
            instance.name = "estrogen(Clone)";
        }

        private void SpawnEBox()
        {
            var player = GameObject.Find("PLAYER");

            var mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mesh.transform.localScale = new Vector3(.2f, .1f, .1f);
            GameObject.Destroy(mesh.GetComponent<Collider>());

            var instance = new GameObject("estrogen box(Clone)");
            mesh.transform.parent = instance.transform;
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.position = player.transform.position;
            var instanceColl = instance.AddComponent<BoxCollider>();
            instanceColl.size = new Vector3(.2f, .1f, .1f);
            instance.AddComponent<Rigidbody>();
            instance.AddComponent<EstrogenBox>();
            instance.MakePickable();
        }

        private AssetBundle LoadBundle()
        {
            var bytes = new byte[0];

            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream("EstrogenMod.Resources.e.unity3d"))
            {
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
            }

            return AssetBundle.CreateFromMemoryImmediate(bytes);
        }
    }
}