using System;
using System.Collections.Generic;

namespace HowlEngine.ECS;

public class StructPool<T> where T : struct{
    private struct Slot{
        public T Data;
        public ushort Gen;
        public bool Active;

        public Slot(){
            Gen = 0;
            Active = false;
        }
    }

    private Slot[] _slots;
    private Stack<int> _freeSlots;

    /// <summary>
    /// Creates a new StructPool.
    /// </summary>
    /// <param name="size">The length of the internal array of this StructPool.</param>
    public StructPool(int size){
        _slots = new Slot[size];
        _freeSlots = new Stack<int>();
    }

    /// <summary>
    /// Assigns a free slot in the StructPool to be available for usage.
    /// </summary>
    /// <returns>A new handle to access the data within the slot.</returns>
    public Handle Allocate(){
        if(_freeSlots.Count == 0){
            Console.WriteLine($"[StructPool]: Max Amount of {typeof(T).Name} ({_slots.Length}) reached!");
        }
        
        // get the latest free slot.
        int index = _freeSlots.Pop();
        Slot slot = _slots[index];
        
        // set the slot to active to be put into the game loop.
        slot.Active = true;
        // increment the generation to avoid reuse and id bugs.
        slot.Gen += 1;

        return new Handle(index, slot.Gen);
    }

    /// <summary>
    /// Removes an available slot in the StructPool, freeing it for later resauge. 
    /// </summary>
    /// <param name="handle">The specidifed Handle to access the slot.</param>
    public void Free(ref Handle handle){
        // get the slot.
        Slot slot = _slots[handle.Id];
        
        // remove the slot from the game loop.
        slot.Active = false;

        // append the slot to free slots for reuse.
        _freeSlots.Push(handle.Id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle">The specified Handle to access the slot.</param>
    /// <returns>A reference to the data stored in the slot.</returns>
    public ref T Get(ref Handle handle){
        return ref _slots[handle.Id].Data;
    }
}