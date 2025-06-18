internal class GameHelpers
{
    private static CharacterMovement localMovement = null;

    internal static CharacterMovement GetMovementComponent()
    {
        if (localMovement != null)
        {
            return localMovement;
        }
        if (Character.localCharacter == null
        || Character.localCharacter.gameObject == null
        || Character.localCharacter.gameObject.GetComponent<CharacterMovement>() == null)
        {
            return null;
        }
        localMovement = Character.localCharacter.gameObject.GetComponent<CharacterMovement>();
        return localMovement;
    }

    private static Character localCharacter = null;

    internal static Character GetCharacterComponent()
    {
        if (localCharacter != null)
        {
            return localCharacter;
        }
        if (Character.localCharacter == null)
        {
            return null;
        }
        localCharacter = Character.localCharacter;
        return localCharacter;
    }

    private static CharacterRagdoll localCharRagdoll = null;

    internal static CharacterRagdoll GetRagdollComponent()
    {
        if (localCharRagdoll != null)
        {
            return localCharRagdoll;
        }
        if (Character.localCharacter == null || Character.localCharacter.gameObject.GetComponent<CharacterRagdoll>() == null)
        {
            return null;
        }
        localCharRagdoll = Character.localCharacter.gameObject.GetComponent<CharacterRagdoll>();
        return localCharRagdoll;
    }
}