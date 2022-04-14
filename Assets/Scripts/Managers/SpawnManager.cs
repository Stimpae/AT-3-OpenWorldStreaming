using System;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public class SpawnManager : Singleton<SpawnManager>
    {
        public GameObject playerPrefab;
        public CinemachineVirtualCamera mainCamera;
        
        public void SpawnPlayer(Vector3 centrePosition, float radius)
        {
            // do a while loop until a space is found thats on land
            // then spawn the character and break it.. this needs to be after the map generation in
            // the execution order
            while (true)
            {
                Vector3 spawnPosition = centrePosition + Random.insideUnitSphere.normalized * Random.Range(0, radius);
                if (Physics.Raycast(spawnPosition, -Vector3.up, out var hitPoint))
                {
                    if(hitPoint.transform.gameObject.CompareTag("Land"))
                    {
                        Vector3 position = new Vector3(spawnPosition.x, 0, spawnPosition.z);
                        GameObject go = Instantiate(playerPrefab, position, Quaternion.identity);

                        mainCamera.Follow = go.transform;
                        mainCamera.LookAt = go.transform;
                        break;
                    }
                } 
            }
        }
    }
}
