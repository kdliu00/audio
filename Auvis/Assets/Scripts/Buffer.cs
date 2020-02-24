using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buffer : List<float> {

    int capacity;

    public Buffer(int c) {
        capacity = c;
        for (int i = 0; i < c; i++) {
            Add(0);
        }
    }

    public new void Add(float value) {
        base.Add(value);
        if (Count > capacity) {
            RemoveAt(0);
        }
    }
}
