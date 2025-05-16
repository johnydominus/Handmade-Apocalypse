using TMPro;
using UnityEngine;
#nullable enable

public class UIWInLossMessage : MonoBehaviour
{
    public TextMeshProUGUI messageHeader;
    public TextMeshProUGUI messageText;

    public void SetMessage(bool win, ThreatType? fatalThreat)
    {
        if (win)
        {
            messageHeader.text = "!!! YOU WIN !!!";
            messageText.text = $"Congratulations! You sucsessfully decreased all the global threats and saved the humanity from annihilation! For now.";
        }
        else
        {
            messageHeader.text = "!!! YOU LOST !!!";

            switch (fatalThreat)
            {
                case ThreatType.Hunger:
                    messageText.text = "Food on the planet gone almost zero. Billions died of starving. " +
                        "Those few who left spend days fighting for crumbles." +
                        "The earth is not fertile anymore and there is no animals to hunt. The humanity is lost.";
                    break;
                case ThreatType.Asteroid:
                    messageText.text = "The giant cosmic rock crashed the Earth, the cradle of mankind, our beloved home." +
                        "Waves of flame embraced the planet. Oceans boiled. The atmosphere dissappeared." +
                        "The collision wiped any possible life. Nothing could survive. The humanity is lost.";
                    break;
                case ThreatType.Pandemic:
                    messageText.text = "The virus killed almost all the population. Goverments failed. Anarchy rules the world." +
                        "Nobody living knows enough to fight the virus." +
                        "Those few who survived live in chaos of constant struggle and fear. The humanity is lost.";
                    break;
                case ThreatType.ClimateChange:
                    messageText.text = "The Earth, our native planet, that used to be so kind to us, became absolutely inhabitable" +
                        "Nature disasters roaming the surface of land and water, huge territories are swallowed by the ocean," +
                        "temperatures are extreme and unstable. There's no place to hide and there is nowhere to live. The humanity is lost.";
                    break;
                case ThreatType.NuclearWar:
                    messageText.text = "Thousands of missiles brought hell on Earth. Billions were wiped by nuclear fire." +
                        "Lands and waters are poisoned by the radiation. Those few who survived live in hate, fear and struggling. The humanity is lost";
                    break;
                case ThreatType.DarkAges:
                    messageText.text = "People fell to easy choices and trap of illusions. Knowledge is lost. " +
                        "Nothing new invents and even the exisitng technologies is not used anymore - nobody knows how." +
                        "Sectants, populism and interactive influencers rule the masses. The humanity is still alive. But it is lost.";
                    break;
                default:
                    messageText.text = "You didn't make it. The humanity is lost.";
                    break;
            }
        }
    }
}
