using Imaginarium.Ontology;

public class Character
{
    // todo: change directory to application persistent datapath.. copy over from resources
    public Ontology Ontology = new Ontology("Characters", "Assets/Scripts/Imaginarium");

    // todo: change void to ?
    public void MakeCharacters()
    {
        var character = Ontology.CommonNoun("character");
        var invention = character.MakeGenerator(10).Generate();

        foreach (var c in invention.PossibleIndividuals)
        {
            // c.whatever
        }
    }
}