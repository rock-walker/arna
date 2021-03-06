USE [AutoPortalDb]
GO
INSERT [dbo].[Address.City] ([ID], [Name], [GoogleCode], [Ru]) VALUES (N'66bb51a3-dc2b-40c2-96cf-fbd6e3fcf2de', N'Minsk', N'ChIJ02oeW9PP20YR2XC13VO4YQs', N'Минск')
INSERT [dbo].[Address] ([ID], [CityID], [Street], [Building], [Comments], [Country], [SubBuilding], [Apartment], [SubApt]) VALUES (N'205357e5-f1d5-4a83-b9ea-627ad9e1abea', N'66bb51a3-dc2b-40c2-96cf-fbd6e3fcf2de', N'Карвата', N'73', NULL, N'Belarus', N'k7', NULL, NULL)
INSERT [dbo].[Address] ([ID], [CityID], [Street], [Building], [Comments], [Country], [SubBuilding], [Apartment], [SubApt]) VALUES (N'b0e298ee-59c3-4ad1-b890-b9c1e20ae0a5', N'66bb51a3-dc2b-40c2-96cf-fbd6e3fcf2de', N'пр. Жукова', N'44', N'', N'Belarus', N'', 0, N'')
INSERT [dbo].[Markers] ([ID], [Lat], [Lng]) VALUES (N'cab02c54-f231-437c-82ca-39078d034404', 53.909104, 27.684351)
INSERT [dbo].[Markers] ([ID], [Lat], [Lng]) VALUES (N'd204a313-a7de-417b-8ca6-6d9a80e08780', 53.876119, 27.517392)
SET IDENTITY_INSERT [dbo].[AutoBrand] ON 

INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (1, N'All', N'all')
INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (2, N'Audi', N'gr')
INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (3, N'Citroen', N'fr')
INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (4, N'Peugeout', N'fr')
INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (5, N'BMW', N'gr')
INSERT [dbo].[AutoBrand] ([ID], [Brand], [Country]) VALUES (6, N'MINI', N'gr')
SET IDENTITY_INSERT [dbo].[AutoBrand] OFF
INSERT [dbo].[Contacts] ([ID], [Mobile], [Municipal], [Email], [Chat], [Web]) VALUES (N'27795954-bbd5-41c0-b314-558b7a6c6b0a', N'8 (029) 339-83-68;8 (029) 567-71-77', NULL, NULL, NULL, N'http://alexpremium.by/                                                                              ')
INSERT [dbo].[Contacts] ([ID], [Mobile], [Municipal], [Email], [Chat], [Web]) VALUES (N'bb8d8dae-f194-4dde-a537-5cf228590d9a', N'8 044 7113112', N'', N'', N'viber:8 044 7113112', N'www.s-auto.by                                                                                       ')
INSERT [dbo].[Workshop] ([ID], [Name], [ShortName], [BrandName], [AddressID], [LocationID], [ContactID], [LogoID], [AutoBrandID]) VALUES (N'33cdb48f-094e-4beb-99a8-8562c38ae96d', N'Алекс премиум авто', N'APA', N'AlexPremiumAuto', N'205357e5-f1d5-4a83-b9ea-627ad9e1abea', N'cab02c54-f231-437c-82ca-39078d034404', N'27795954-bbd5-41c0-b314-558b7a6c6b0a', NULL, 5)
INSERT [dbo].[Workshop] ([ID], [Name], [ShortName], [BrandName], [AddressID], [LocationID], [ContactID], [LogoID], [AutoBrandID]) VALUES (N'515e51a7-8423-4d14-afcd-ff7a42b9d868', N'ЗауберАвто', N'', N'BMW ЗауберАвто', N'b0e298ee-59c3-4ad1-b890-b9c1e20ae0a5', N'd204a313-a7de-417b-8ca6-6d9a80e08780', N'bb8d8dae-f194-4dde-a537-5cf228590d9a', NULL, 5)
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (1, 0, N'Carservice', N'carservice')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (2, 0, N'GasStation', N'gas')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (3, 1, N'Wires', N'wires')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (4, 1, N'Suspension', N'suspension')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (5, 1, N'Electric', N'electric')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (6, 1, N'Interior', N'interior')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (7, 1, N'Exterior', N'exterior')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (8, 1, N'Gear', N'gear')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (9, 0, N'History', N'history')
INSERT [dbo].[Categories] ([ID], [Parent], [Title], [Link]) VALUES (10, 1, N'Diagnostic', N'diagnostic')
SET IDENTITY_INSERT [dbo].[Categories] OFF
/*
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'42d9802e-b8a1-4a93-b402-183dfed65bf1', CAST(N'2015-02-04T21:18:22.450' AS DateTime), 7, 4, NULL)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'cab00de4-3a7c-4f14-8fc8-26f7c394b143', CAST(N'2015-02-05T19:12:13.623' AS DateTime), 7, 4, NULL)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'30e9dec0-9b7d-4543-b887-552310430401', CAST(N'2015-03-05T20:36:52.417' AS DateTime), 7, NULL, 10)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'a10a2aab-e89f-428d-957f-5be9d9cd9c85', CAST(N'2015-01-29T20:02:11.970' AS DateTime), 7, 4, NULL)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'77007f64-d2bf-4fe1-87c3-a04571d59421', CAST(N'2015-02-05T19:36:06.927' AS DateTime), 7, NULL, 10)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'2e7f3af1-c1b3-40c5-978a-ab16090d321d', CAST(N'2015-02-01T11:21:27.290' AS DateTime), 7, 4, NULL)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'74dc68b6-01d2-4dd9-bf9c-b96f98dde806', CAST(N'2015-03-03T20:43:18.033' AS DateTime), 7, NULL, 10)
INSERT [dbo].[CalendarDay] ([Id], [UtcDate], [Category_Id], [Entrepreneurs_Id], [Movable_Id]) VALUES (N'9a537929-85f2-47db-8960-e5b227bcb6fe', CAST(N'2015-02-04T20:09:30.177' AS DateTime), 7, 4, NULL)
*/
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'2a977032-f9c8-4be0-99a7-170004dac08f', N'a10a2aab-e89f-428d-957f-5be9d9cd9c85', 12, 30, 12, 45, 0, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'58b79bd8-1509-4f60-a866-48ed70ae0075', N'42d9802e-b8a1-4a93-b402-183dfed65bf1', 12, 30, 12, 45, 0, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'e426128a-ba8a-4df5-9bfc-503135bbe310', N'30e9dec0-9b7d-4543-b887-552310430401', 12, 30, 12, 45, 1, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'77489d82-6b78-40b2-b4be-5879ef5fb31b', N'2e7f3af1-c1b3-40c5-978a-ab16090d321d', 12, 30, 12, 45, 0, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'15a49551-4250-4715-ad01-5d49f8c4ccac', N'9a537929-85f2-47db-8960-e5b227bcb6fe', 12, 30, 12, 45, 1, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'f5632aac-e5a1-4f5c-9b24-6ee2ed60da22', N'77007f64-d2bf-4fe1-87c3-a04571d59421', 12, 30, 12, 45, 1, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'89a402ae-098b-4f5d-9ffc-c76141f0c327', N'74dc68b6-01d2-4dd9-bf9c-b96f98dde806', 12, 30, 12, 45, 1, NULL, NULL)
INSERT [dbo].[Reservations] ([Id], [CalendarDayId], [StartH], [StartM], [EndH], [EndM], [Status], [WorkshopId], [UserId]) VALUES (N'b49058dd-34d0-436b-aff0-f4ab469c6cd7', N'cab00de4-3a7c-4f14-8fc8-26f7c394b143', 12, 30, 12, 45, 1, NULL, NULL)
SET IDENTITY_INSERT [dbo].[Gallery] ON 

INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'dc433b81-a5b7-47a0-899c-3a5b508c5573', N'33cdb48f-094e-4beb-99a8-8562c38ae96d', 10, 0)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'2dc013ca-635f-44cb-b8f3-434ca5e64766', N'515e51a7-8423-4d14-afcd-ff7a42b9d868', 10, 2)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'f6eb191d-e86c-4e2d-94e5-7db56f08dc61', N'33cdb48f-094e-4beb-99a8-8562c38ae96d', 6, 1)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'18b1439c-e816-4d68-b041-7eb8f3f1b4c3', N'515e51a7-8423-4d14-afcd-ff7a42b9d868', 5, 2)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'658dd26b-c4eb-4664-a4d0-b9023140f6db', N'515e51a7-8423-4d14-afcd-ff7a42b9d868', 6, 0)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'be71ef78-5e49-4156-8337-df68bb9cd84c', N'33cdb48f-094e-4beb-99a8-8562c38ae96d', 7, 1)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'e3d85fd0-b60f-4e67-bef0-e6f5f4263442', N'33cdb48f-094e-4beb-99a8-8562c38ae96d', 4, 2)
INSERT [dbo].[WorkshopCategories] ([ID], [WorkshopID], [CategoryID], [MomentBookingState]) VALUES (N'da7ae48a-41b6-4eec-9d01-f9e104532e6b', N'33cdb48f-094e-4beb-99a8-8562c38ae96d', 5, 0)
