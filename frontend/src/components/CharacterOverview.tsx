import React from 'react'
import { useNavigate } from 'react-router-dom';
import { Character } from '../types'

const CharacterOverview = ({characters}: {characters: Character[]}) => {
    const navigate = useNavigate();
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 2xl:grid-cols-4 gap-8 w-fit mx-auto">
            {characters.map((character: Character) => {
                return (
                    <div title={`Go to detail page of ${character.name}`} onClick={() => navigate(`/character/${character.id}`)} key={character.id} className="col-span-1 w-90 rounded-lg shadow-lg p-4 border bg-white border-gray-200 cursor-pointer">
                        <img className="aspect-square w-82 object-contain" src={character.imageUrl !== '' ? character.imageUrl : 'https://static.wikia.nocookie.net/disney/images/7/7c/Noimage.png'} alt={character.name} />
                        <p className="font-medium pt-3 text-disney-blue">{character.name}</p>
                    </div>
                )
            })}
        </div>
    )
}

export default CharacterOverview;