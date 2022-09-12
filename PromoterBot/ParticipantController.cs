using Deployf.Botf;

namespace PromoterBot
{
    public class ParticipantController : BotController
    {
        [Action("Add a participant")]
        public async Task Add()
        {
            await Send("Hello from other controller!");
        }
    }
}
