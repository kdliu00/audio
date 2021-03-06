using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Instantiate512cubes : MonoBehaviour {

    // Prefab cube that gets spawned in.
	public GameObject cubePrefab;
    public float radius = 150f;

	// Array that holds the 512 cubes we're spawning in.
	GameObject[] cubes = new GameObject[512];

    // Scales the height of each cube by this much.
	public float scale = 100;
    private float sped;
    private Color colorCycle;
    private float inc = 0.01f;
    private float colorSum = 0;

	/* Spawns 512 instances of cubePrefab in a circle of radius 100
     * around the object this script is attached to. Each cube is
     * rotated to face towards/away the center, and each cube is a
     * child of the object this script is attached to.
     */
	void Start () {
        sped = 0;
        colorCycle = new Color(1, 0, 0, 1);
		for (int i = 0; i < 256; i++) {
            // Spawns a copy of cubePrefab.
            GameObject cube = Instantiate(cubePrefab);
            GameObject cubeTwin = Instantiate(cubePrefab);

			// Assigns this copy to it's proper position in cubes.
			cubes[i] = cube;
            cubes[i+256] = cubeTwin;

			// Names it properly.
			cube.name = "Cube" + i;
            cubeTwin.name = "Cube" + (i+256);

            // Set its parent to this object.
            cube.transform.parent = this.transform;
            cubeTwin.transform.parent = this.transform;

            /* Rotate and position the cube properly. Some attributes that
             * might come in handy include Transform.eulerAngles, Transform.forward,
             * and the Transform class in general. Make sure you're using floats
             * if you plan on doing any division.
             */

            float angle = 360 * (i + 0.5f) / 512;

            cube.transform.eulerAngles = new Vector3(0, -90 + angle, 0);
            cubeTwin.transform.eulerAngles = new Vector3(0, -90 - angle, 0);

            cube.transform.position = this.transform.position + 
                cube.transform.forward * radius;
            cubeTwin.transform.position = this.transform.position + 
                cubeTwin.transform.forward * radius;
		}
	}
	
	/* Every frame, we'll take the data collected in SpectrumAnalyzer
     * and use it to set the heights of our cubes. Each of the 512 data
     * points our sample array corresponds to one of our cubes. Two caveats:
     *     1. FFT values are very small, so you'll want to scale each one up
     *        (use the scale variable).
     *     2. If a FFT value is 0, you don't want the cube to disappear, so
     *        add a small base height to every cube.
     * Hint: You can access public static variables using Class.variable.
     */
	void Update () {
        float sum = 0.2f * SpectrumAnalyzer.audioBands[0] + 0.8f * SpectrumAnalyzer.audioBands[1];

        float targetScale = 1f + (sum * sum * 0.4f);
        
        //transform.localScale = Vector3.Lerp(transform.localScale, 
            //new Vector3(targetScale, 1, targetScale), Time.deltaTime / 0.1f);

        colorCycle.a = transform.localScale.x / 2f;
        
        /*if (colorSum >= 3) {
            inc = -0.01f;
        }
        if (colorSum <= 0) {
            inc = 0.01f;
        }
        colorSum += inc;

        if (colorSum <= 1) {
            colorCycle.r = colorSum;
            colorCycle.g = 0;
            colorCycle.b = 0;
        } else if (colorSum > 1 && colorSum <= 2) {
            colorCycle.r = 1;
            colorCycle.g = colorSum - 1;
            colorCycle.b = 0;
        } else if (colorSum > 2 && colorSum <= 3) {
            colorCycle.r = 1;
            colorCycle.g = 1;
            colorCycle.b = colorSum - 2;
        }*/

        GameObject floor = GameObject.FindWithTag("Floor");
        if (floor != null)
        {
            floor.transform.localScale = new Vector3(
                transform.localScale.x * 100, transform.localScale.x * 100, 1);
            floor.GetComponent<MeshRenderer>().material.color = colorCycle;
        }

        transform.Rotate(new Vector3(0, sped * Time.deltaTime, 0));
		for (int i = 0; i < 256; i++) {
			if (cubes != null) {
                GameObject cube = cubes[i];

                Vector3 target = new Vector3(1, Mathf.Max(1f, SpectrumAnalyzer.samples[i] * scale), 1);

                cube.transform.localScale = Vector3.Lerp(cube.transform.localScale, target, Time.deltaTime / 0.1f);

                //twin time
                GameObject cubeTwin = cubes[i+256];

                cubeTwin.transform.localScale = Vector3.Lerp(cubeTwin.transform.localScale, target, Time.deltaTime / 0.1f);
			}
		}
	}
}