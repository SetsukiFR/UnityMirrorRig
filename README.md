**MirrorRig for Unity**
A tool for Unity, generating in-editor tools for easy transform mirroring.

**First Installation**
 - Add the scripts to your project
 - Go In *Project Settings/Mirror Rig Settings*
 - Objects that will be mirrored are the ones beginning, or ending, with the given strings. (Examples are provided.) For instance, `hand_L` will mirror with `hand_R`

**First Application**
 - For a rigged character : Add the MirrorRig component to the root (or the armature)
 - The MirrorRig "pairs" array should autofill with all the mirrored objects and their required data. For instance, `Hand` will contain references and data for `Hand_L` and `Hand_R`
 - When editing a mirrorable object (For instance, `Hand_L`), you should notice, in the Transform inspector, a "Mirror" checkbox.
 - Check it (or press M) to begin mirror edition.
 
Note : Multi-object edition should also works.