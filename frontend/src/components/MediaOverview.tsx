import React from 'react'
import { useNavigate } from 'react-router-dom';
import { Media } from '../types'
import { img_base } from '../consts'
import Details from './Details';

const MediaOverview = ({medias}: {medias: Media[]}) => {
    const navigate = useNavigate();
    return (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 2xl:grid-cols-4 gap-8 w-fit mx-auto">
            {medias.map((media: Media) => {
                return (
                    <div title={`Go to detail page of ${media.name}`} onClick={() => navigate(`/${media.mediaType === 'Movie' ? 'movie' : media.mediaType === 'TV' ? 'series' : 'shorts'}/${media.id}`)} key={media.id} className="col-span-1 w-90 rounded-lg shadow-lg p-4 border bg-white border-gray-200">
                        <img className="aspect-square w-82 object-contain" src={media.posterPath !== null ? `${img_base}${media.posterPath}` : 'https://static.wikia.nocookie.net/disney/images/7/7c/Noimage.png'} alt={media.name} />
                        <p className="font-medium pt-3 text-disney-blue">{media.name}</p>
                        <Details name="Type" value={media.mediaType === 'Movie' ? 'Movie' : media.mediaType === 'TV' ? 'Series' : 'Short'} />
                        <Details name="Release Date" value={media.releaseDate} />
                        <Details name="Score" value={`${media.voteAvg}/10 (${media.voteCount})`} />
                    </div>
                )
            })}
        </div>
    )
}

export default MediaOverview;