using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HowlEngine.Collections;

public class ClassPool<T> : IDisposable where T : class, IDisposable{
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
    /// Gets or the total number, both active and inactive, of elements the internal data structure.
    /// </summary>
    public int Capacity => _slots.Length;

    /// <summary>
    /// Gets or the total number of active elemnts in the internal data stucture.
    /// </summary>
    public int Count {get; private set;} = 0;

    /// <summary>
    /// Creates a new StructPool.
    /// </summary>
    /// <param name="size">The length of the internal array of this StructPool.</param>
    public ClassPool(int size){
        _slots = new Slot[size];
        _freeSlots = new Stack<int>(Enumerable.Range(0,size));
    }

    /// <summary>
    /// Assigns a free slot in the StructPool to be available for usage.
    /// </summary>
    /// <returns>A new token to access the data within the slot.</returns>
    public Token Allocate(){
        if(_freeSlots.Count == 0){
            Console.WriteLine($"[WARNING]: Max Amount of {typeof(T).Name} ({_slots.Length}) reached!");
            return new Token();
        }
        
        // get the latest free slot.
        int index = _freeSlots.Pop();
        ref Slot slot = ref _slots[index];
        
        // set the slot to active to be put into the game loop.
        slot.Active = true;

        Count++;
        return new Token(index, slot.Gen);
    }

    /// <summary>
    /// Removes an available slot in the StructPool, freeing it for later resauge. 
    /// </summary>
    /// <param name="token">The specidifed Token to access the slot.</param>
    public void Free(ref Token token){
        Free(token.Id);
    }

    /// <summary>
    /// Removes an available slot in the ClassPool, freeing it for later resauge. 
    /// </summary>
    /// <param name="token">The specidifed Token to access the slot.</param>
    public void Free(int id){
        if(_freeSlots.Count == _slots.Length){
            Console.WriteLine($"[WARNING]: StructPool {typeof(T).Name} has reached zero!");
            return;
        }
        // get the slot.
        ref Slot slot = ref _slots[id];
        
        // remove the slot from the game loop.
        
        slot.Active = false;

        // null the strong reference, preparing it for garbage collection.

        slot.Data = null;

        // increment the generation to avoid reuse and id bugs.
        
        slot.Gen += 1;

        Count--;

        // append the slot to free slots for reuse.
        
        _freeSlots.Push(id);
    }

    /// <summary>
    /// Calls the Dispose function of a given class instance within the internal data structure.
    /// </summary>
    /// <param name="token">The token used to index the instance within the internal data structure.</param>
    public void DisposeAt(ref Token token){
        DisposeAt(token.Id);
    }

    /// <summary>
    /// Calls the Dispose function of a given class instance within the internal data structure.
    /// </summary>
    /// <param name="id">The id used to index the instance within the internal data structure.</param>
    public void DisposeAt(int id){
        if(_freeSlots.Count == _slots.Length){
            Console.WriteLine($"[WARNING]: StructPool {typeof(T).Name} has reached zero!");
            return;
        }
        // get the slot.
        ref Slot slot = ref _slots[id];
        
        // null the strong reference, preparing it for garbage collection.
        slot.Data.Dispose();
    }

    /// <summary>
    /// Gets the data within the internal data structure.
    /// </summary>
    /// <param name="token">The specified token to try and recieve data from.</param>
    /// <returns>A RefView with a reference to retrieved data.</returns>
    
    public RefView<T> TryGetData(ref Token token){
        ref Slot slot = ref _slots[token.Id];
        if (slot.Gen == token.Gen){
            return new RefView<T>(ref slot.Data, true);
        }
        else{
            return default;
        }
    }


    /// <summary>
    /// Gets the data within a slot.
    /// </summary>
    /// <param name="slotIndex">The specified index in the internal array to access the slot.</param>
    /// <returns>A reference to the data stored in the slot.</returns>
    public T GetData(int slotIndex){
        return _slots[slotIndex].Data;
    }


    /// <summary>
    /// Checks if a slot is currently free or active.
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <returns>true if the slot is currently active; otherwise false.</returns>
    public bool IsSlotActive(int slotIndex){
        return _slots[slotIndex].Active;
    }

    /// <summary>
    /// Frees the internal data structure and calls Dispose on all stored instances.
    /// </summary>
    public void Dispose(){
        Parallel.For(0, _slots.Length, i =>{
            _slots[i].Data.Dispose();
        });
        _slots = null;
        _freeSlots.Clear();
    }
}