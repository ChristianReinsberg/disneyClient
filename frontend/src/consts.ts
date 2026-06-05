import { MediaType } from "./types";

export const img_base = 'https://media.themoviedb.org/t/p/w342';

export const to: Record<MediaType, string> = {
    'Character': 'character',
    'Movie': 'movie',
    'Short': 'shorts',
    'TV': 'series',
}