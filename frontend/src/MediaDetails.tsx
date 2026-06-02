import React, { useEffect, useState } from 'react';
import { useParams, useLocation } from 'react-router-dom';
import { apiService } from './services/api';
import { MediaDetails } from './types';
import { img_base } from './consts'
import CharacterOverview from './components/CharacterOverview';

const MediaDetail = () => {
    const location = useLocation();
    const {id} = useParams<{id: string}>();
    const [media, setMedia] = useState<MediaDetails | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        const fetchDetails = async () => {
            try {
                if (id) {
                    if (location.pathname.includes('movie')) {
                        const data = await apiService.getMovieDetails(parseInt(id));
                        setMedia(data);
                    } else if (location.pathname.includes('series')) {
                        const data = await apiService.getSeriesDetails(parseInt(id));
                        setMedia(data);
                    } else if (location.pathname.includes('shorts')) {
                        const data = await apiService.getShortDetails(parseInt(id));
                        setMedia(data);
                    }
                    
                }
            } catch (error) {
                    console.error('could not load media details', error);
            } finally {
                setLoading(false);
            }
        }
        fetchDetails();
    }, [id]);

    if (!media) {
        return <p className="text-disney-blue font-medium text-2xl">Media not found</p>
    }

    return (
        <div>
            {loading ? (<p className="text-disney-blue text-2xl font-medium">Loading media details</p>) : 
            (
                <div>
                    <section className="flex justify-evenly mb-8">
                        <article>
                            <h2 className="text-disney-blue text-2xl font-medium">{media.title}</h2>
                            <p className="text-disney-blue max-w-5xl mt-4">{media.overview}</p>
                        </article>
                        <img className="aspect-square w-90 object-contain" src={media.posterPath !== null ? `${img_base}${media.posterPath}` : 'https://static.wikia.nocookie.net/disney/images/7/7c/Noimage.png'} alt={media.name} />
                    </section>
                    <section>
                        <h2 className="text-disney-blue text-2xl font-medium text-center mb-4">Characters</h2>
                        <CharacterOverview characters={media.characters} />
                    </section>
                </div>
            )}
        </div>
    )
}

export default MediaDetail;