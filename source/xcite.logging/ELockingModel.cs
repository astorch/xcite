namespace xcite.logging {
    /// <summary> Locking model enumeration </summary>
    public enum ELockingModel {
        /// <summary> Flag to set that the file is locked from the start of logging to the end. </summary>
        Exclusive,
        
        /// <summary> Flag to set that the file is only locked for the minimal amount of time. </summary>
        Minimal
    }
}