using System;
using System.Collections.Generic;

namespace HowlEngine.Audio;

public class AudioManager{

    /// <summary>
    /// Gets the FMOD Studio System created by this AudioManager.
    /// </summary>
    public FMOD.Studio.System StudioSystem {get; private set;}
    
    /// <summary>
    /// Gets the FMOD Core System created by this AudioManager.
    /// </summary>
    public FMOD.System CoreSystem {get; private set;}
    
    /// <summary>
    /// Handles to loaded banks are stored via their name without the ".bank" extension.
    /// </summary>
    private Dictionary<string, FMOD.Studio.Bank> _loadedBanks = new Dictionary<string, FMOD.Studio.Bank>();
    
    /// <summary>
    /// Handles to loaded events are stored via their path without the full relative path (just their event name).
    /// </summary>
    private Dictionary<string, FMOD.Studio.EventDescription> _loadedEvents  = new Dictionary<string, FMOD.Studio.EventDescription>(); 
    
    /// <summary>
    /// Handles to buses are stored via their name without the "bus:/" extension.
    /// </summary>
    private Dictionary<string, FMOD.Studio.Bus> _busHandles = new Dictionary<string, FMOD.Studio.Bus>(); 
    
    /// <summary>
    /// The full system path to the directory where all audio banks are stored.
    /// </summary>
    private string _folderPath;


    /// <summary>
    /// Creates a new AudioManager instance.
    /// </summary>
    /// <param name="fileDirectory">The file directory path, relative to the executable, where all audio files are located.</param>
    public AudioManager(string fileDirectory){
        // Set the folder path in relation to the current platform.

        _folderPath = System.IO.Path.Combine(AppContext.BaseDirectory, fileDirectory);
        

        // load Masters.

        CreateFmodSystems();

        // Use Stereo Audio.
        
        HandleResult(CoreSystem.setSoftwareFormat(
            48000,
            FMOD.SPEAKERMODE.STEREO,
            0 // <-- should never change.
        ));

        // initialise with all defined settings as it cannot be changed afterwards.

        HandleResult(StudioSystem.initialize(
            128, // how many simultaneous audio channels (voices) FMOD can mix and play at the same time.
            FMOD.Studio.INITFLAGS.NORMAL,
            FMOD.INITFLAGS.NORMAL,
            0
        ));

        // load master banks.
        LoadBank("Master");
        LoadBank("Master.strings");
        LoadBusHandle("Master");
        SetBusVolume("Master", 1.0f);
    }


    /// <summary>
    /// Prints the result of an FMOD function call.
    /// </summary>
    /// <param name="result"></param>
    private void PrintResult(FMOD.RESULT result){
        System.Console.WriteLine($"[AUDIOMANAGR]: {result}");
    }

    /// <summary>
    /// Prints the result of an FMOD function call, if it is not RESULT.OK.
    /// </summary>
    /// <param name="result"></param>
    private void HandleResult(FMOD.RESULT result){
        if(result != FMOD.RESULT.OK){
            PrintResult(result);
        }
    }

    /// <summary>
    /// Creates instances for FMOD Studio and Core System.
    /// </summary>
    private void CreateFmodSystems(){
        HandleResult(FMOD.Studio.System.create(
            out FMOD.Studio.System system
        ));

        HandleResult(system.getCoreSystem(
            out FMOD.System coreSystem)
        );

        StudioSystem = system;
        CoreSystem = coreSystem; 

    }

    /// <summary>
    /// Disposes of the FMOD Studio and Core System.
    /// </summary>
    public void Dispose(){
        System.Console.WriteLine($"[AudioManager] Dispose");
        HandleResult(CoreSystem.release());
        HandleResult(StudioSystem.release());
    }

    /// <summary>
    /// Loads a bank instance into the FMOD Studio System to play sounds from.
    /// </summary>
    /// <param name="bankName">The name of a bank to load, without the ".bank" extension.</param>
    public void LoadBank(string bankName){
        
        // Load the bank from FMOD studio.
        
        HandleResult(StudioSystem.loadBankFile(
            GetBankPath(bankName),
            FMOD.Studio.LOAD_BANK_FLAGS.NORMAL,
            out FMOD.Studio.Bank bank)
        );
        _loadedBanks.Add(bankName, bank);
        
        // Get all event descriptions from the bank. 
        
        bank.getEventList(out FMOD.Studio.EventDescription[] events);
        foreach(FMOD.Studio.EventDescription e in events){
            e.getPath(out string path);
            path = System.IO.Path.GetFileNameWithoutExtension(path);
            _loadedEvents.Add(path, e);
        }
    }

    /// <summary>
    /// Unloads a bank instance from the FMOD Studio System.
    /// </summary>
    /// <param name="bankName">The name of a bank to load, without the ".bank" extension.</param>
    public void UnloadBank(string bankName){

        // Get loaded bank.
        FMOD.Studio.Bank bank = _loadedBanks[bankName];
        
        // Remove all loaded event descriptions.

        bank.getEventList(out FMOD.Studio.EventDescription[] events);
        foreach(FMOD.Studio.EventDescription e in events){
            e.getPath(out string path);
            path = System.IO.Path.GetFileNameWithoutExtension(path);
            _loadedEvents.Remove(path);
        }

        // Unload the bank from FMOD Studio.
        
        _loadedBanks[bankName].unload();

        // Remove the unloaded bank.

        _loadedBanks.Remove(bankName);
    }

    /// <summary>
    /// Loads the handle to access a bus within the FMOD Studio System.
    /// </summary>
    /// <param name="busName">The name of a bus handle to load, without the "bus:/" prefix.</param>
    public void LoadBusHandle(string busName){
        
        // Get the bus.

        StudioSystem.getBus("bus:/"+(busName == "Master"? "" : busName), out FMOD.Studio.Bus bus);
        ref FMOD.Studio.Bus b = ref bus;
        _busHandles.Add(busName == ""? "Master" : busName, bus);
    }

    /// <summary>
    /// Sets the volume of a bus, via a bus handle that has been loaded.
    /// </summary>
    /// <param name="busHandleName">The name of the loaded bus handle to use when accessing a bus in the FMOD Studio System.</param>
    /// <param name="volume">The specified volume to change to.</param>
    public void SetBusVolume(string busHandleName, float volume){
        _busHandles[busHandleName].setVolume(volume);
    }

    /// <summary>
    /// Gets the volume of a bus, via a bus handle that has been loaded.
    /// </summary>
    /// <param name="busHandleName">The name of the loaded bus handle to use when accessing a bus in the FMOD Studio System.</param>
    /// <returns></returns>
    public float GetBusVolume(string busHandleName){
        _busHandles[busHandleName].getVolume(out float volume);
        return volume;
    }

    /// <summary>
    /// Plays a one-shot instance of a sound.
    /// </summary>
    /// <param name="eventName">The name of the event to play, excluding its relative path.</param>
    public void PlayOneShot(string eventName){
        
        // Get the loaded event description.

        FMOD.Studio.EventDescription desc = _loadedEvents[eventName];
        
        // Create and play an instance of the description.

        desc.createInstance(out FMOD.Studio.EventInstance inst);
        inst.start();
        
        // Immediately release it, so when the sound has finished, FMOD Studio can garbage collect it.
        inst.release();

        // update the audio system to play the sound.
        StudioSystem.update();
    }

    /// <summary>
    /// Gets the full relative file directory path to a bank within this project.
    /// </summary>
    /// <param name="bankName">The specified name of the bank to get the path to.</param>
    /// <returns></returns>
    private string GetBankPath(string bankName){
        // NOTE: this will need to change when building to other platforms like consoles.
        // string folderPath = HowlApp.Content.RootDirectory+@"\bin\DesktopGL\Audio\";
        string filePath = System.IO.Path.Combine(_folderPath, bankName+".bank");
        System.Console.WriteLine(filePath);
        return filePath;
    }
}