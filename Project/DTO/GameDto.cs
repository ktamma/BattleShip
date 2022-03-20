using System;
using DAL;
using Domain;

namespace DTO
{
    public class GameDto
    {
        private Game game;


        public GameDto(Game game)
        {
            this.game = game;
        }

        public void Save()
        {

                using var db = new ApplicationDbContext();
                var gameDto = game.GetSaveDto();
                db.Games.Add(gameDto);
                db.SaveChanges();
            
        }
        
        
    }
}