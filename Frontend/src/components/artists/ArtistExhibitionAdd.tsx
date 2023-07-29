import {
	Autocomplete,
	Button,
	Card,
	CardActions,
	CardContent,
	IconButton,
	TextField,
} from "@mui/material";
import { Container } from "@mui/system";
import { useCallback, useContext, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { BACKEND_API_URL } from "../../constants";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import axios, { AxiosError } from "axios";
import { debounce } from "lodash";
import { Artist } from "../../models/Artist";
import { Museum } from "../../models/Museum";
import { SnackbarContext } from "../SnackbarContext";
import { Exhibition } from "../../models/Exhibition";
import { getAuthToken } from "../../auth";


export const ArtistExhibitionAdd = () => {
	const navigate = useNavigate();
	const openSnackbar = useContext(SnackbarContext);

	const { artistId } = useParams();
	const [artistName, setArtistName] = useState("");
    const [museums, setMuseums] = useState<Museum[]>([]);
	const [exhibition, setExhibition] = useState<Exhibition>({
		artistId: 0,
        museumId: 0,
        startDate: new Date(),
        endDate: new Date(),
	});

	const addExhibition = async (event: { preventDefault: () => void }) => {
        event.preventDefault();
        try {
            await axios
                .post(`${BACKEND_API_URL}/exhibitions`, exhibition, {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`,
                    },
                })
                .then(() => {
                    openSnackbar("success", "Exhibition added successfully!");
                    navigate("/artists");
                })
                .catch((reason: AxiosError) => {
                    console.log(reason.message);
                    openSnackbar(
                        "error",
                        "Failed to add exhibition!\n" +
                            (String(reason.response?.data).length > 255
                                ? reason.message
                                : reason.response?.data)
                    );
                });
        } catch (error) {
            console.log(error);
            openSnackbar(
                "error",
                "Failed to add exhibition due to an unknown error!"
            );
        }
    };

	useEffect(() => {
        const fetchArtists = async () => {
            try {
                const response = await axios.get<Artist>(
                    `${BACKEND_API_URL}/artists/${artistId}`,
                    {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`,
                        },
                    }
                );
                const data = response.data;
                setArtistName(data.firstName + " " + data.lastName ?? "");
                setExhibition({
                    ...exhibition,
                    artistId: data.id ?? 0,
                });

                const response2 = await axios.get<Museum[]>(
                    `${BACKEND_API_URL}/museums/0/10`,
                    {
                        headers: {
                            Authorization: `Bearer ${getAuthToken()}`,
                        },
                    }
                );
                const data2 = response2.data;
                setMuseums(data2);
            } catch (error) {
                console.log(error);
            }
        };
        fetchArtists();
    }, []);

	const fetchSuggestions = async (query: string) => {
        try {
            const response = await axios.get<Museum[]>(
                `${BACKEND_API_URL}/museums/search?query=${query}`,
                {
                    headers: {
                        Authorization: `Bearer ${getAuthToken()}`,
                    },
                }
            );
            const data = response.data;
            const removedDupes = data.filter(
                (v, i, a) => a.findIndex((v2) => v2.name === v.name) === i
            );

            setMuseums(removedDupes);
        } catch (error) {
            console.error("Error fetching suggestions:", error);
        }
    };

	const debouncedFetchMuseumSuggestions = useCallback(debounce(fetchSuggestions, 250), []);

	useEffect(() => {
		return () => {
			debouncedFetchMuseumSuggestions.cancel();
		};
	}, [debouncedFetchMuseumSuggestions]);

	const handleMuseumInputChange = (event: any, value: any, reason: any) => {
		if (value.length < 3) return;
		console.log("input", value, reason);

		if (reason === "input") {
			debouncedFetchMuseumSuggestions(value);
		}
	};

	return (
		<Container>
			<Card>
				<CardContent>
					<IconButton component={Link} sx={{ mr: 3 }} to={`/artists`}>
						<ArrowBackIcon />
					</IconButton>{" "}
					<h2>Add exhibitions to {artistName}</h2>
					<form id="addExhibitionForm" onSubmit={addExhibition}>
						<TextField
                            id="artistName"
                            label="Artist Name"
                            value={artistName}
                            variant="outlined"
                            fullWidth
                            sx={{ mb: 2 }}
                            disabled={true}
                        />
						<Autocomplete
							id="museumName"
                            sx={{ mb: 2 }}
                            options={museums}
                            getOptionLabel={(option) => option.name}
                            renderOption={(props, option) => {
                                return (
                                    <li {...props} key={option.id}>
                                        {option.name}
                                    </li>
                                );
                            }}
                            renderInput={(params) => (
                                <TextField
                                    {...params}
                                    label="Museum Name"
                                    variant="outlined"
                                />
                            )}
                            filterOptions={(x) => x}
                            onInputChange={handleMuseumInputChange}
                            onChange={(event, value) => {
                                if (value) {
                                    console.log(value);
                                    setExhibition({
                                        ...exhibition,
                                        museumId: value.id ?? 0,
                                    });
                                }
                            }}
                    	/> 
                        <TextField
                            id="startDate"
                            label="Start Date"
                            variant="outlined"
                            onChange={(event) => setExhibition({...exhibition, startDate: new Date(event.target.value)})}
						/>

                        <TextField
                            id="endDate"
                            label="End Date"
                            variant="outlined"
                            onChange={(event) => setExhibition({...exhibition, endDate: new Date(event.target.value)})}
						/>
					</form>
				</CardContent>
				<CardActions sx={{ mb: 1, ml: 1, mt: 1 }}>
					<Button type="submit" variant="contained" id="addExhibitionForm">Add Exibition</Button>
				</CardActions>
			</Card>
		</Container>
	);
};