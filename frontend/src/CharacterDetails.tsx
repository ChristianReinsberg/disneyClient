import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { apiService } from './services/api';
import { CharacterDetails } from './types';
import MediaOverview from './components/MediaOverview';

const CharacterDetails = () => {
    const {id} = useParams<{id: string}>();
    const [character, setCharacter] = useState<CharacterDetails | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchDetails = async () => {
            try {
                if (id) {
                    const data = await apiService.getCharacterDetails(parseInt(id));
                    setCharacter(data);
                }
            } catch (error) {
                    console.error('could not load character details', error);
            } finally {
                setLoading(false);
            }
        }
        fetchDetails();
    }, [id]);

    if (!character) {
        return <p className="text-disney-blue font-medium text-2xl">Character not found</p>
    }

    return (
        <div className="mx-auto p-8 mb-8">
            {loading ? (<p className="text-disney-blue text-2xl font-medium">Loading character details</p>) : 
            (
                <div>
                    <section className="flex justify-evenly mb-8">
                        <h2 className="text-disney-blue text-2xl font-medium">{character.name}</h2>
                        <img className="w-90 aspect-square rounded-full" src={character.imageUrl !== '' ? character.imageUrl : 'https://static.wikia.nocookie.net/disney/images/7/7c/Noimage.png'} alt={character.name} />
                    </section>
                    <section>
                        <h2 className="text-disney-blue text-2xl font-medium mb-2 text-center mb-4">Appears in:</h2>
                        <MediaOverview medias={character.medias} />
                    </section>
                </div>
            )}
        </div>
    )
}

export default CharacterDetails;