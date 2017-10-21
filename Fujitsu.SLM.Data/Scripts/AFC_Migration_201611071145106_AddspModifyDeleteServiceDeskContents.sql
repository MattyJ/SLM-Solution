﻿CREATE TABLE [dbo].[Asset] (
    [Id] [int] NOT NULL IDENTITY,
    [FileName] [nvarchar](max),
    [FileExtension] [nvarchar](max),
    [OriginalFileName] [nvarchar](max),
    [FullPath] [nvarchar](max),
    [MimeType] [nvarchar](max),
    CONSTRAINT [PK_dbo.Asset] PRIMARY KEY ([Id])
)
ALTER TABLE [dbo].[ContextHelpRefData] ADD [AssetId] [int]
CREATE INDEX [IX_AssetId] ON [dbo].[ContextHelpRefData]([AssetId])
ALTER TABLE [dbo].[ContextHelpRefData] ADD CONSTRAINT [FK_dbo.ContextHelpRefData_dbo.Asset_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [dbo].[Asset] ([Id])
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201611031620207_AddAssetTable', N'Fujitsu.SLM.Data.Migrations.SLM.Configuration',  0x1F8B0800000000000400ED5D5F6FDC38927F3FE0BE43A39FEE16B36EDB830176037B179E4CB21B6C320EE2CCE0DE0CB95BB175A356F7B6D4D91887FB64F7701FE9BEC2496AFD2159556491A2A46EAF9097B8455691C55F158BFFAAFEEF7FFEF7EACFDFD6F1EC6BB84BA34D723DBF383B9FCFC264B95945C9E3F57C9F7DF9FD1FE67FFED3BFFECBD59BD5FADBECD7BADCF745B9BC66925ECF9FB26CFB6AB148974FE13A48CFD6D172B749375FB2B3E566BD08569BC5E5F9F91F1717178B302731CF69CD66579FF64916ADC3F28FFCCFD79B64196EB37D107FD8ACC238AD7ECFBFDC9554673F07EB30DD06CBF07AFE76FF9F5196EECFEEDE7F38FB29C882F9EC268E82BC1D7761FC653E0B9264930559DECA57BFA4E15DB6DB248F77DBFC8720FEFCBC0DF3725F82380DABD6BF6A8B733B727E597464D156AC492DF769B6595B12BCF8BE92CC42ADEE24DF7923B95C766F721967CF45AF4BF95DCF6FD234CCE63395D3ABD7F1AE2825CBB61C89B3B2CA7733F0E1BB060B39648A7FDFCD5EEFE36CBF0BAF93709FED82BCC4C7FD431C2DFF163E7FDEFC1626D7C93E8EC5F6E52DCCBF493FE43F7DDC6DB6E12E7BFE147EA95AFD6E359F2DE47A0BB562534DA873E8D2BB24FBFE723EFB39671E3CC46133FC42F7EFB2CD2EFC4B9884BB200B571F832C0B774941232C0508B82BBCDE467158FCAFE698632E579EF9EC43F0ED7D983C664FD7F3FCBFF3D9DBE85BB8AA7FA95AF14B12E5BA9657CA76FB90C3E8CDB72C4CD2121E3D73BBDD458F5112C4C3752F1FA25CF84FBD33FA905B9E82857F463F075FA3C712540ACBDCC065E1B7ECAF61BCCD815D98AD743EFB14C665D9F429DA1ECCD7192C775F29EDDBDD66FD6913A3B40E65EE3F07BBC7A2E8E78DA1E0DD66BF5B2AADBF5AB4E6426B4420553B8B02EB4FE645C72B6F8B06A817E7E72CA08206EA997E8EB258A71F5CB67A2E05063EE758E85DE34BD88341E32A6FA581BED5B5D642A3BAD67AEDACAEBBE8619F83D05E4FAB8A9382EA78E56EE6AEE58729CBE51F3C60F8CD3A88E29BD56A17A6A98ED90FBD1884D72572C48E5282D5D37997E4D2CAA59CC3BBB12FC5FF3F476BFB46D5C47ED45AC87E04F2CB7615F8EA47456B906ED02E4A35C2B8A1AB3EDE0B562195AC1C56007A2468A96EEE48D36C2BE356D59A2C1B47E90DEE7F4F3E48C37D93853A8B77C9D40CA38B103D26E1EA66B77C8AB270A9734A7A322937CB2CFADA08FAC74DAE32413259D897636125DBE9CFCA0247526B8BB9ADBD0B775FA365F85398FE86B7562870DF5AE1B6B1D8773023A0853A4D080245BB3941A8384D0B3A5E8584C69D12262FF4C5DA489D17EA647154EBA8354BDC6616B5DF25DB7D569E29A08D958ADC4B56A96D315908184ABA24662D756DFF14A69BB838F1419B5D7FA55A8C7D078D450BD9B6B3AE9BDBEF28D14F416511AAC56421723282253BCD48D2D0D9CD4952D56956D2F11286CB7A7A2031D8C8BED9EA366A3AAC42A9BB5A12982A43F10E1E5D1F164BDF7A4CEF9CB409CAD746A1D4DA934EE95D9B4A5A3FEFD70FC55CDBCDE9AA898DE23BDE6D76D9ED6E65D70B362AEB19CF0E8D75AD09857D59768A541CE5627FBEDD3D06499496AD2CC8F6487E985D2CA50179DD6C18C6D38A8AA635EE8AEA765BA85DFE7B10E75F96619AD26B16BCEC7D6BDC5A3FC05014F8D6A6F2AEAB98BFEC36FBEDC1B1D6AD6690E2C89A0694024E8DA6A8A33BF67AB3DE6E92DC12B2966342697A4DD614A29B0F4A3A3B93B82565AE2DA9CABAF5265EC7D855B262C79E1736D6BAC76D25734FEBB2EC1E36157C2F109CF606CCADEEBA22C04D8B9D2786D398FC32ED0D415466D5C2CADF1A5CCBC6661A331032CE6DDAFA00E74E446C35B69D99FB9DCD999D03B3BF477D765AEE6B494DDA6DAFDD27B47687647E8DD2E8A1BDD7C83B61B7DE02683C4227C4525426B0EA7801A94D3825710A57120EA7E44DED0997DA03EB5A4CEFC3AF61DC0D151F835D4E4895BFE95633D5A09114E4D0FAF2A6539445DA8DA98B1FBC5C32AF58BEDD27CB6244A7AB032F75A3EBF55314AF54F5D09E1BB7BB1FB86E21A7C8A62AD499B2B19EEDFE17D5E2617B4B5CAE604BC9CB3A0750C756386421731FB4AB1AC67E4A6D7A748DAFCB808DB914E9045D98829FA6868FCB766D0F1D5C89BAF2E449E878D552F2714C751317BC83E2B6B1C90DF07A2A55DEAB9926E0973A018B1065D93AB9026DE6C47294B9C60B773D93D1BA0FBE6CB6A947B49577BA4AC7BF4957B700EB0F519277A7AE2DDE69F61187DA69B3052130CD429C5968DA5E2141A9E89ACB0B84B2EA0444ED1B845246A7EC0C79B8CD34B94234AD715DA1169E8CD9562C4CCDB36D19F2410328E8FBF8BFE36D7B7DBB4DB7012CD6D9166F055C3C1C563FA043E4F674A0194D27FF06549F2615F3A432F936241C0D57A99C30CAA33901B7C3B5E61342B42B14EB3B6F3E20A8D09AA06779E3FCC42037B411AD1F9D7E0C96962FD4C59A132A75BC3C1CB717710F133D8AB94F272C3933E213F6A43F05B00EE6B3427E9404BB675E40360756C33C4D998206BCF4D5362B74550138346A005A800CAA2297EAB6B88B82C75DB0B65CD21D2A4DE67F32FFDE3957D81A6806A8B84D93C03409F43E09545843EDBFFA0D0632500B74B2FA1F839C5698D93E156FAA4D96DF7049F62026C302B41FA56AB8FF1AC47BEDD4D153D86671DA2870DF34E8F06532762764EC2C1E9E3C76D98005D5270BA3E37510D709ED6FB171F4395C6FF399D3F56508A83EE148EB917A7F1A0206607A1B629A2F2B894D8F435EC284A95D1D148F438082E0370540B17B42BFC45504BB12B821C9AFE9F6460469F6F09D56D754F6E2E2765AD5696D6F9B3BB61834603F35C5C961D5D5B11DD09A843E84A75A0A72C6FA469726BBA6A9D2E95E2F1C42173F647A5772FC377AEBB19A9E86BCECE9D772E6F56896A9898763CA6D7BA7795F2117B9575B81768B2A4BDA63B282176BEC72A35DAE3B59E263BD7D588FD364815FBA05D65AA77B595F51ABA49621AD1128E8EAEDEAEF757BB5ACD46C6134C59D2CAB9B4D9DAC69C7F3FB9E6DA97818E3664D1BF40D73203F19EF6337DEBA5C189D6C3865F748636FBD63B1F98761B362F38FFBD61E22DB13C2777A43422CE4C5E9CD29BA59E7BCE264A03917C7AB5505A95C7E3200834025C3F0930F792EC6617B39145BD6D18D578ED88B97A179174F1E7AE7290538EC9D9BF372B0DBF2C36A0EA0A62C74A2E8360738E5F6506B4FB381A33E8F7A1A2D65D2F0D3100B451F84A3F9DAC0B48E7861EB08DE51B0B7F343D2549B8F1C79867BBFC62ED81E16E0EFD2B771F09836B2E2D9F0826615FF3A956F51CA765C2CE76ACEF3E15D85BBF8398783A876F2987E088B0C5C55176F56EBC2672E6F995ECFCF0100A4C277CF6916AE9BD21750A807F1893FDEA4E9661995DD6A2E072559F82DFB6B186FEB60F579A17C8864CE6F92D5AC7A30435468D1D05E53856573BDC845176D7361E5237E3DFF1DE8238353B34C6C391DDAAC103F3F3BBB00F47352617123210AE29C419A0F5F946470A28D9265B40D62665394FACCA9BA18A58693FAE5A7701B26C52CCB943BA7096505BC1D0D3BC5993089EB6A2160CA0035340D3639FAFA9CD802C89ABBFFF2E85FA822B9BA4DF2693FD7F7D94DB958CC7904E9325841439BEBCECAAE5D082485EF66D4BBA15227A22120A9130587BFF87A6714488AD1691A1C51238F15C6002945D0E19B3B943C86AB21F0EE84475D0F0680A36E7C4E028D8684B31470B8D9675B0829998BF9186566AE1558C1B4AEA639DA5960A2DEF13A8086B1F22E262C0416CF5C8CAFD2C69E0CA0D7C631E3B44109CF378A7EDF6E1DF2575118732186811CAF608376A796208AA097C3D1A946976E0FA0355DF0C1699E96D0B1A958B3D56B8961984C6000C581B9565BA6ED9EB5C76954934D956A3A27B5AAB9D53AE13032B2221C60922E8F72E226DC3078F8FCEC1BC0491076EAAC1716FC14277CB6FE76582C1B39DC72823B5A9C16D1A99EC65CFD220956D850D2194827F070780D6517B5790D0CCD36243900526A6F30582BB63E4582C00A8DAB7F6C6E15A753C369BF761839CD50F3BB8CA9E89AC4165C8869B25CB021DDD3C6AC4512A52EF3AA175093621C01D9A49C2C96D5E243EB51377241E475D36E2B1D861DEED05457DAECB774C910EEE266130C097EA4A699ECCE807BBCD4B0719A20E7983802B872362E19F1F7BD03F694372E8D3D191CAC27BC7169C8B1608526E4510E13B8FDFA0DC6C4571D5C767F0806D21B0BC6404036586E5E858F82E676CDCADDB8216BD86C761996BB340F2FFB321DE443A51F608B8B22E07DABD0C44F378919525F1CDBFC66DBE701EC84EDB05BCD7E38A9A3B1216A9E046BDDA82BF6AD130D1FB32E80DC1BBDDA188DEF8B15EE4F4E27E8ECEA3A318AE29F9E8B8B8793A75063882D0F2F6B1D928258DC7DD586A53F89DB60DA2E0C804AED1871F88F7F1F4C8D6E4DA1850E752DEC2FD559092CAE315101B24F027F54EB07801E351E27813A7EC4400A370EE1035B40218F52F8887588D668C1D9DF69B7753B0700ADF5A871DAA489603B2AB87591C84CE062852583B01AF838CC2AB66527F5EBA4010C610E887D86B038ADC1E20F8F0A77103CC4841A3A920884CAC080A60355C1A6714F403A419892D580B8A5646203D6D1412A850930A1008F19001150067AB17720F0B0340363BF132AB11E0C88486C7C4E0A8D64DC391374CC41E8B876AA67436A8E43CA77623C9B54930C0704B2494A36A01EF9E08B1100C0681719D1001023ECB0316B114CC0C67D3D22036DECD890E6DA38AE36381F7A9DF7A68C1F51BC42CE6BB40112DE7F28CE2DAA87F34884885FD2B00A129156213854301664EFC24C7C3E9FCE676F9A701572F4018065B93A160D0190C20A31E836CFE9718ACD3B7413A96AF309A5D3ECF4198808DBF9181DE924C5404A7AFA8711539E681AC8C1C7A880202C62A0D9DA3640ABFD64A0413D250214A9824EF4692118CA332502DFE290128245791813E3C7534013A6021E5121C43045B375830C24D12BF7802A5A8AA96575345052CF2AD7D2A469F0FA29543658866B0A0CB72B682361A868C71F9C689BF8820A4CFB599C39E96CE8E138D03424875304D4ECD5873906124D84238C8890B1D4A8D1201321A2CAA08C812AB2F50AC9220E1D93AC4E8DE17286499456377529C724A823C5267288C24BD229F740B8A4DA1444343D6A5E13DC339DDB5345989A09C549EF078B4625AD138CF1A89A9EE21E18704F2D025009A46BDF505D33C822E1880B8D9284C9CA1C4E49EE8D36A09228A5D609D409471B0749117ADB8FCEF24143F620E23187F691BAA30DEE23F446766E35F2D186F3E148DB413686003288946C42CE48DD63069D113AAAFAF21AD931C3CC08C48D6E7B57614AAB16931CC94B6D9A5E6277DB7C480FBBD7C6C4B383D09C429C20F2EC1E2A451249A7602982B478EBAFEEAC9181B25B93791BB976AE670F12FE8A9C2314F09ABC17D18377E40217D2B5B117A826AA06224B6E0C0EA9838C281C8CBE71496AC4655CBF3BCFF3C6601BF4D46F17A7039BBCD9913AA041151735662F811D9BC386910FA1EBD49F193E42DF5F9DCA7714E5407AAE8D12418B8D115502EB9E3EAE04149EB00A36CB4E1F494220CED9B0EA2E4964FCCDE234C436D0769B8E6EE04BB074508261341B3E90D72C9DF48FE9D1D50EF99C1E7139EBAD14C6228A7C402F7AC8C6DDC9AE5233AC01988FBA351D34AC019C0537EC1AC0F05A982B39E2C685B99BF0C6854719C25B1216A6A18B17C9717C982F5B71878FE3DCD8B990637931EC07AC0C11F21EBF6AFB6F7CFEDA55C0C6F7AE98AEDB1DACF81B84E685A485F0F157952C99807795BE840D1E52D242369D22F9102E3E25999F591AFA8A4F448E821B64F6C11FF269F6CF352FFED00D6FFCCD1FB2A35B1D0232F6D0F1577E7D6D12ABAFCAB0ED4CEDCB3379B7917A7B26BA66CD49A66EDB927A6DD6931CF8EF9C1001393E92426FB8F19F490992C00E4F35C2B57F1865C5AB83F8752F713482673FE041C5C079C2830880B7E6B3797933949461F6585AB4FAC7226857C9E72248FF78C2235F7920140967BE83B8E48C85B4A8E8270B68A7D0470B48870EE7F90C29A1CF143832EF201B3A213B2D27DE657AB48BC6EBF47C40D8D1D788D3E7028F71295B0740E6556E1C3DE6CBDC1838DB1B221C849AAF6FFBB281751EB9E61E70F3ED6A71B77C0AD741F5C3D5222FB20CB7D93E88CB849D69FDE143B0DD46C963DAD6AC7E99DD6D8365E1B1FDFE6E3EFBB68E93F47AFE9465DB578B455A924ECFD6D172B749375FB2B3E566BD08569BC5E5F9F91F1717178BF581C66229E15DBDB5DC70CA36BBE03154BE16F72856E1DB689766C51AE2212872DEBD5EAD4131E5D63371EBA766265D6C86A3595F00AA8B17FFAF2F36B6C9F20A7E67E83D9D56806FF33EADF3012DBB17AA176760BDBCE6DD3288831D92BCF4F526DEAF13FAFE3B5DFB6D1497ED9769B4BFDA517AF32D0B9322312024277CE2D3BCDD458F5112C4782BE1578BD6EEE3F863903D290D6D7EE553FA10ADC3C3F25DA4D4FE0A295D2D1408A8305B009C291AAFA2968569E4AE57277C9BE931C0CE21D20FF2FF163ECBD5CB1FF8F53F4759AC0C7AF5139F46D1EDCFE5530C914CFB2B9FD24D9DC85024744365371C15839A9B7996E8AB1F71B8C18EACDD0FDE7E490F214C440AF56F7C2A6F72DF2CBE59AD76619ACAB4E42F7C8A627815919E2EEC8A4646524A64495AD2177B8A3F3EE3F47EB4D25A29D3B13414E2076B7A6AE3849F8F47F79AADA26E8A47EC8071B48EACDA8FCAD5FCA0EB207F71A0B829979028C90DBAA2A769E6563A7A4CC2D5CD6EF91465E13203465CFD6C413B5F957D55FA5EFF3629F50B506AE9ACA0935E6B0E4418AAADADDD8F7617BCA066B7BF4E53E0A42D8AB62877C63BE98BFCD0D65E630CF5FBD11925B0A848C8107374B431633CC1B018351335C6C09949F433760DDF9FF7EB87E294523605CA4717BAC0982A9F2C70B6D965B7BB95DA4AE1E7A3C197E68EAE05AE282A0C3CD1558FCF061869D251C9513EFC20E64EBC1137DD5CDA997371ABC5CC512835F9142FC0A7209EE174B2263C9A0CDBC225D48FA53166449676F36DD3271FD9887B714FAC483B8FFFC08E0BDE08E43C4753AE5FD783A2F56B94460FEA2E7FF3E3D100927C74E6C5A53151B57071CCA4FA4120E00FC147149970A7C11DE76591FD0E14796183BF0DA521D1D34E73CDF07DF8358C955D24E51B9F2A9D1D5BA4CFCFA1CD683FB2552E7FB2F68ECBEDE6288B08B758FC6C4D5B0C278DD0D6459BD62061F2B58FD5D7663C84B2B7374DC82367734353E8C7DAA829B4E52B2CFAF4DA34D59B380B7749501C0E412B003EDAAF929B50AFD8E2988C03AB91E1A4A7C7AAA7D86BE84EAACA20C8505716957E55166A96FC65F23735B0529EAA7A39EE3C04A7733FF024EAF774E42925EA968E3DB529BC698A7D1B7D9F9BCC93C13F5A830F430D743B5B3591E39CAF9A69F4A9A4509BC4DF2733AF0113F345B617F36FC7C3625EB025DCF3591FDE1A0852568517790E6C78A2EE136D06DAF62833121C045D752B8CA8920B4EB650834AF9E97C27104AA1B6ED21A7AFDE0FC090DD54EB4DD4E2755002575DCDAF7DBEF4A1377697BF1D025FC81BB9F5AF7694903B07C2CFD335C8C95B57BDF53AFA443727BD0AB9EFE09A5335272B62B5D03888111A12E983353DC49CC85F268B325914C5A20869353AD994360387BD55D1D4EDC7AE340CA1C3AB7C72A0F96B10EF29A2D5378BB7AAC0DED8DA9A49F78E56F7609E994E2A6824C75045068D7E54F2C018EAA3F8FB8BDCBD60057DB24081317A0A03050C1AFDA0A0CFDB38684647781D8795F891D10308E5A3BC8FA3C6015285827D9F269F1730F970025839989D0E7772CC244EE5841F2649C6546ABA3AC3A6774A0AE5E592839282CF5D99C6B8E60015C9ED04B5EE03A54093EA30E89D92EA78529A0EEA3264F4363FBB9275CB919D02E98BC3EC05B627E52F93EABD20D52BE3B47AD1BE2245ABBB02A2B5FB3D04AFB2DC92979B3B3C6FD03E6E70A02BAF812F50F2A04C472E970C2E97C7B29CD5DC82615F97E97891C27881C2F9FD9BE6DD5BFF6ED5D8F6A909D5EBC748B9477D3093E8D75C61F10F8027612CDC09E2467E6D2147A8631BC1E0B33DE4C9BDBF8EBB7E938333BE832307D356EE4D18B24973A3452155ADA2431551C5D525332B41349426CFC2A8E4305817826D5AD5B9C155B875D706DBB6EFF5265945C598CFDEA53FEFE3F87AFE2588532A1A122D063518BB35C46EB786A4C3D6C12384BA6ED14190C13370EA08B7868C0798995A7A8C38D3F4BF33BE74399819B120B06ADC7856C8E898732B7784504BCF0396CCC9A58F12463A1974C693BA8EE3592A4DAD0E6822A9761C1E40D70396E8B6769CAA7B81124302FE2C1388E1C1324CB0966D6C109DC603017443544DD7A751024D3C4620D11DF78E1F985ED30246B0B28F598E20DE71A428AA3D808BCA217A5418E3CBC38C399041492DD22C47AB5F9ABF9B0C4A55F62229AD5229A02249522998B4CAA4A4A6333A1499CF72697C8D56652AA3E7340BD7D51CFAF7F8751C9596AE2EF02148A22F619A7DDEFC1626D7F3CBF38BCBF9EC268E82F49024AB4AD4F46A595EBF0E9264935529B418999B2EBE2F323785ABF542AD6E9FFFA9A092A6AB18C9FE540C5505172411D2D5DF42008A1A2CF9AA6C46E1EA6AA156BC42B059F0BE9E47CDE4F1973029560FE1EA6390152FE18B5261D9CAF9AC805FF05024F6AA20B8D0926F73141D98245F83DDF229D8FDDB3AF8F6EF22B56C07AFF662C4847C4A1E28C2544A3E9AD96453F240AC7D20614D4CDC12D202CE948CE864D157A63692E5369FA96AFCEA5DB20ABF5DCFFF2B37AEB9D1FCFB3E7C35FB9C0B73F6DFF3D987E0DBFB3079CC9EAEE717E7E7D6FC3F1F1223A92DA0A97210D1A649F200AF265592300AB4808A3AAF66EFFEE3BEAAF6DDACBC0FFC6A765E08CB1B16D10C45270BC23AD9910E05977FB01D3839ED918EF60FF6B815DF29D901A3AD496183D50079DFFFD08462733C8B0A1BED48EC47C4187414947400D0B199C2F67FA756F2550D4D4B74B27A26E7361AD1EA2B1991348379F983B5F547D22279C6749D27E940F6A138719FB4F7F8B4570827F54214B84D61C47798A6E96C5208EC64F585A8841233CE0EBC52E58EF8454E80ED1A8390E861F5A0CF5174B23000598E045ECEA4C6769384E7A4DCEEB07180EF529FECF81F8919300449EBD0348A605FCDEDC535C76EF1F5C167F2267CB612BF2E6087638C067F762391249F3B3BA99648C2438BD0C3A54E3A2F50EAC11FE0DCF43AD95901EF9C9039CA6E600CE43A9962E96A979B6E0D099097E53DE27D3C45FF0F126922577277ABAC5D487DEEA693050591F769C2038507FD35AA93C5811A17C65DF4743E263B834FD1E9E2BE28C1633CEF2F226FEEF44EAAEBD2420C23E3E475B504A6FDCCA35B81B8FB46BDB846DA244F276BEFD43C517602976B77D2219064848F2A9B055B1325C76D9556559FACC5B19F7E18733C9DBCC64E0E2AEF4018095974B2632FA777B2336162DD53B0D4A39FF14D56BAFF336A7D72A613D7D3C94233AFEC58E4483A5948B0322A9D16565C075A9BA6E8A50CB09CDCE8B40676482340E7133A592474DE3A6C23F7F14FCA5984A9D7339D41D6E62C3A10CEE93E4449B07B06AF30D8E47AB92B305D7DFCA7702BB1C4429339391D7322652EF26251E4D4459351998C8AAD5121F20A9DAC5951721379F2501D50A1E633D229A7CBA353C140B9DD109E94AB77E532640A3A592513130E9DD61A903D7486F43E273B74BE6F82A041415DAE822084FE99EF82603985ECC40A294CDED8B14F18FAF43E276B74F487BB5DC36F8054416E7A32DD8238352D798187E0BD6A88BB6E4C5A713A5AF142F481B5BDD7451BBAAEA0E5443F5EB6F3263D19504F40169D93551525138F2C46A78061200B8F3F9A20F54E5FA42F7D92A6577A5DA8E29975FCD33F64D4F14057C9A7E381E2C0BE89BD9578A13106A89C37FD6C94C09C377C3ED6B8EC8381876D2E8B0DAEC959D0D3EACD5910222637FB886AD4D47B2C72EF9B6435FBB429B81DBE56CD2BE2129F55BF7CD8C759B48DA365CEF77A7E7E7606D2CE09549060AD2249ECB34CFF7780785E302C365AA320CEABA7D92E7716407EA18FBB285946DB20967AA394621ABB42CC0D3DF5CB4FE1364C0A2B467798C3B5896C0A59371C140B6C92831437DB008EEAA0FB5E88679A92C86822314A23D9FC288FDF0508BD7E9BE45634CCC2D9CDF210D6FA75902E8315D49922C6B80E5775E0551550CDEFBD20098F43D91F98D0F0B2043BF1AAC32840122EF8DFE3823A321C897109C56648BF9F3C8EC8E88BC78A234322366A34F1646FD590C28F5633991CB04F24AC7CE9052D7A11F4831A6D763B9C212F05DDE000A2A38F76B2043DD9A47191666B2B060799F2026D1478DD6ED18475F8CF26ABA5AF258EBFA1E420E8C4DBC068664F78B590793F08267ACBE1AC6DFBB1819BD8416A91212795ABB0D0FE6835DB1E1BCA3439A48F095086A49C0380479383D2841B10BB0AC3112C64852B2B8C7AC68E3E36573F58B2C22D371FE7700BC93620241E67C9E4CD29E90965974EF8688521B5FA216997057DBF9E1A992AB1C795A4DC770E5F3ADED671E0CB38BBF589A831ACD218E8B1B246CC84AEC381A53E4596E22A9178C142DC88C38B7E1F72CF4B3D141781DC7EEB057BC6F03FFDC04F1B238C60A946E03A0A00AAAA41EFD57718EB7ED1E76647FDDA3E2B20789E386D4CA078777ED46DFDF2628E10AA88841D8C1C236D63C1AF836EF64BF78BA44DBEEA4B2FA03344D3E91576D80D6382A11CC4EA08E0765A9BB7E3626CE0CD5B7B741DC3E62D0AAFDAC61AA752DB917DB14E9CFDE08FE0BE81C0A2E36E8AB1F72F7CEEA51E83C735E4FEA993AB2586353E0A8C50A1C8CC73A03E5E1B3A3D1AAA0C62D146DC04B18A71D70F685DB64674A9AD8E0AC375B4352E76D510741ACC82A2A7B2F1C60AB7777C589372291D09C84E6261302AD486590EB8E16AC4958018DDF034EE2B4AF118B1761C3E9CFC8D453AEE24C16FFC2B8B871066A701A33A049FB42B57FF76F2E041030C1E2B6EC0731A2A8E0C092858521C56E4AB958B04EA83E36E23074F103248A41F2CE1DDE730D6C4151A1568CD2912681ABDF705E2B960E33FF0415247D87B06E59067496E8A8045621A15884AC41A33FAD0011E186DC8F6AFFAA9579C0D892F8BAD5DF1F9F3A8A0FAB4F9C73D2EAB23055411BE016B46F9FB8B8112085271EC38AA4E8754A3693652D606A2677C394DD99E9136DC1991D3740C23AF8D6BC2EA0DAEC196007D9B386CEB0B7E7C798B06AB0DB1B1560A6FCA0802C553E3BC46B86BE205ACC2B7D12ECD8A4DE9872085936951EB2ECCC407ECF3D99B261C811C34E06EF914AE83EBF9EA6193A3E010CFA0FA04569E32612C4400E08215C25862E518FC9B17E528E3E62BC5B1296062D56C02413ECD279449F3D5C041DAF6064CA4AF181FA9808195F256113053BE63EC94220686F07D2FE0098B606C612903E7D684018EED278C53FBD5C0817AEB04F8510531EE5459A7B6D0423794E7B7CC7638E0532172786051DD70C1D23CB513E65F4AF784221A05144AF138B77E1EC5B82DA1E1DB1632B0452FB703D668298C3D5A9069EA2A9F9B3476D5779DB9AB8A980C1EBCD50A8D1E2C831A3E588C6BD80D3739689B6FA8A89D0E0C75EDDA0E4EF44D6D0615186D05759853F3E1788D9C9E0F9F7553F4A184094AF5510C0450FD05854DFDD140BECD95021908DF3016C267A31556734660E61794C1ED2E2866608EAC780073A40CC61C29C664AE31BAB0888E35DBECAA1B0D245F9DD553CB30796AB8E9F9B039943B622493F2AB8E4F5980CB8AF6E860112D53CAC3131663BAB54E15A46D261427973C584037693D2EADD172AEC4124C3EAD26975F2501C6826A2177952306341C19260373DC32B933CA02EBD00562F104C4A0AC029BFE53CB3BFB8EA3E1B3907E9BC36C79EA36B25E2C2BEB1682F6DD36447B420460131F4AEA10B5482C7B655AF84994D0F56D4946BF6CED2A1E49F426C9682E80751BE59144E11477079152F7F83D9200584BE25220562B5B060F06F1FEC4DE4E6E6C09533BA2E84549A16BD43C7A3422D2C43D41A4C38D92820A86DAD7900465DAA37017B9FBBC668CE6414F7576814030EB069602A289239D7B2DA5C34D280B6A3E44A7533A66548B9EC433108AF0300CB43418611BA46E68F6AECA1E3136A33001ABAB4151BEE402AFBB9060A80086A40CF1057C75CF1F0E3BF8DAE04DBBC6DFD6BF7F977D226A37F0E0179936F850FF4C5AD54B1E1ABE54EF2A1483ABC97CA6EDD5D51C5914ED3510AE3C888B233E3A35A295A11FB4EA9C1D8B49DBDD091CDA8EB0DF6D3224C37BF3892B146BE35FD635AB6DFB41A77AE343420B61E28F0F7542244E2030E119CE1146161A6EB7CD0FE8BC9AECBEBB8D3FE2D2EC166A5E7BF9DA2D444E8BA4DAE82990C3CE90FAE608DB10D2BE4BF2D461E5C0EAE0E310A751F6DDE43F9141FAEFF8BEC674BD4EE8A8F184484F0BAC22CD274EEE22D43DFED0088FFD6604ED2AEA94980EB77C0E40078181470A1A29E91F34A01D42FBC11205E22D1A4EEEDC85205DAAD70880BE7CEFB9F3C251A054193BE373EF3679075C2302DEBD713F23EA4FD9BA6083BEADAC430AF38E73EF6618F54E8CA7B7B4B8EA8C5CCD7DDAE6DBD5E270505CFD90FF996D76C163F861B30AE3B4FCF56AF1699F1409C50E7FE5AE55F4D892B8CA692661397E2DD1BACCBBE4CBA6BE53ACB4A82EA2E420FB1066C12AF7596F7659F4255866D57E78943CCE67BF06F13E2FF266FD10AEDE25B7FB6CBBCF8AB3E2F543FC2C0AA3B88EACE37FB5006DBEBADD960AE1A30B7933A32291DA6DF2E33E8A574DBBDF2299D20812C53DE72A2F61319659919FF0F1B9A1F4738E241EA14A7CCDF5EC1A43E96D72177C0D5DDAF64B1ABE0F1F83E573FEFBD76855E09022621E0859EC579577965634DAFAF99F398657EB6F7FFA7F603E800590290200 , N'6.1.3-40302')
GO

ALTER PROCEDURE [dbo].[spDeleteServiceDeskContents]
	-- Add the parameters for the stored procedure here
	@ServiceDeskId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN

		DECLARE @ResolverId int
		DECLARE Resolver_cursor CURSOR FOR SELECT Id FROM Resolver
			WHERE ServiceDeskId = @ServiceDeskId

		UPDATE [dbo].[Resolver]
		SET [ServiceComponent_Id] = NULL
 		WHERE ServiceDeskId = @ServiceDeskId

		OPEN Resolver_cursor
		FETCH NEXT FROM Resolver_cursor INTO @ResolverId

		WHILE @@FETCH_STATUS = 0
		BEGIN
				DELETE FROM [dbo].[OperationalProcessType]
				FROM [dbo].[OperationalProcessType] opt
				WHERE opt.Resolver_Id = @ResolverId

				FETCH NEXT FROM Resolver_cursor INTO @ResolverId
		END

		CLOSE Resolver_cursor
		DEALLOCATE Resolver_cursor

		DECLARE @DomainId int
		DECLARE Domain_cursor CURSOR FOR SELECT Id FROM ServiceDomain
			WHERE ServiceDeskId = @ServiceDeskId

		OPEN Domain_cursor
		FETCH NEXT FROM Domain_cursor INTO @DomainId

		WHILE @@FETCH_STATUS = 0
		BEGIN

			EXEC [dbo].[spDeleteServiceDomain] @DomainId;

			FETCH NEXT FROM Domain_cursor INTO @DomainId
		END

		CLOSE Domain_cursor
		DEALLOCATE Domain_cursor

		DELETE FROM [dbo].[ServiceDomain]
			WHERE ServiceDeskId = @ServiceDeskId
	END
END
GO


INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201611071145106_AddspModifyDeleteServiceDeskContents', N'Fujitsu.SLM.Data.Migrations.SLM.Configuration',  0x1F8B0800000000000400ED5D5F6FDC38927F3FE0BE43A39FEE16B36EDB830176037B179E4CB21B6C320EE2CCE0DE0CB95BB175A356F7B6D4D91887FB64F7701FE9BEC2496AFD2159556491A2A46EAF9097B8455691C55F158BFFAAFEEF7FFEF7EACFDFD6F1EC6BB84BA34D723DBF383B9FCFC264B95945C9E3F57C9F7DF9FD1FE67FFED3BFFECBD59BD5FADBECD7BADCF745B9BC66925ECF9FB26CFB6AB148974FE13A48CFD6D172B749375FB2B3E566BD08569BC5E5F9F91F1717178B302731CF69CD66579FF64916ADC3F28FFCCFD79B64196EB37D107FD8ACC238AD7ECFBFDC9554673F07EB30DD06CBF07AFE76FF9F5196EECFEEDE7F38FB29C882F9EC268E82BC1D7761FC653E0B9264930559DECA57BFA4E15DB6DB248F77DBFC8720FEFCBC0DF3725F82380DABD6BF6A8B733B727E597464D156AC492DF769B6595B12BCF8BE92CC42ADEE24DF7923B95C766F721967CF45AF4BF95DCF6FD234CCE63395D3ABD7F1AE2825CBB61C89B3B2CA7733F0E1BB060B39648A7FDFCD5EEFE36CBF0BAF93709FED82BCC4C7FD431C2DFF163E7FDEFC1626D7C93E8EC5F6E52DCCBF493FE43F7DDC6DB6E12E7BFE147EA95AFD6E359F2DE47A0BB562534DA873E8D2BB24FBFE723EFB39671E3CC46133FC42F7EFB2CD2EFC4B9884BB200B571F832C0B774941232C0508B82BBCDE467158FCAFE698632E579EF9EC43F0ED7D983C664FD7F3FCBFF3D9DBE85BB8AA7FA95AF14B12E5BA9657CA76FB90C3E8CDB72C4CD2121E3D73BBDD458F5112C4C3752F1FA25CF84FBD33FA905B9E82857F463F075FA3C712540ACBDCC065E1B7ECAF61BCCD815D98AD743EFB14C665D9F429DA1ECCD7192C775F29EDDBDD66FD6913A3B40E65EE3F07BBC7A2E8E78DA1E0DD66BF5B2AADBF5AB4E6426B4420553B8B02EB4FE645C72B6F8B06A817E7E72CA08206EA997E8EB258A71F5CB67A2E05063EE758E85DE34BD88341E32A6FA581BED5B5D642A3BAD67AEDACAEBBE8619F83D05E4FAB8A9382EA78E56EE6AEE58729CBE51F3C60F8CD3A88E29BD56A17A6A98ED90FBD1884D72572C48E5282D5D37997E4D2CAA59CC3BBB12FC5FF3F476BFB46D5C47ED45AC87E04F2CB7615F8EA47456B906ED02E4A35C2B8A1AB3EDE0B562195AC1C56007A2468A96EEE48D36C2BE356D59A2C1B47E90DEE7F4F3E48C37D93853A8B77C9D40CA38B103D26E1EA66B77C8AB270A9734A7A322937CB2CFADA08FAC74DAE32413259D897636125DBE9CFCA0247526B8BB9ADBD0B775FA365F85398FE86B7562870DF5AE1B6B1D8773023A0853A4D080245BB3941A8384D0B3A5E8584C69D12262FF4C5DA489D17EA647154EBA8354BDC6616B5DF25DB7D569E29A08D958ADC4B56A96D315908184ABA24662D756DFF14A69BB838F1419B5D7FA55A8C7D078D450BD9B6B3AE9BDBEF28D14F416511AAC56421723282253BCD48D2D0D9CD4952D56956D2F11286CB7A7A2031D8C8BED9EA366A3AAC42A9BB5A12982A43F10E1E5D1F164BDF7A4CEF9CB409CAD746A1D4DA934EE95D9B4A5A3FEFD70FC55CDBCDE9AA898DE23BDE6D76D9ED6E65D70B362AEB19CF0E8D75AD09857D59768A541CE5627FBEDD3D06499496AD2CC8F6487E985D2CA50179DD6C18C6D38A8AA635EE8AEA765BA85DFE7B10E75F96619AD26B16BCEC7D6BDC5A3FC05014F8D6A6F2AEAB98BFEC36FBEDC1B1D6AD6690E2C89A0694024E8DA6A8A33BF67AB3DE6E92DC12B2966342697A4DD614A29B0F4A3A3B93B82565AE2DA9CABAF5265EC7D855B262C79E1736D6BAC76D25734FEBB2EC1E36157C2F109CF606CCADEEBA22C04D8B9D2786D398FC32ED0D415466D5C2CADF1A5CCBC6661A331032CE6DDAFA00E74E446C35B69D99FB9DCD999D03B3BF477D765AEE6B494DDA6DAFDD27B47687647E8DD2E8A1BDD7C83B61B7DE02683C4227C4525426B0EA7801A94D3825710A57120EA7E44DED0997DA03EB5A4CEFC3AF61DC0D151F835D4E4895BFE95633D5A09114E4D0FAF2A6539445DA8DA98B1FBC5C32AF58BEDD27CB6244A7AB032F75A3EBF55314AF54F5D09E1BB7BB1FB86E21A7C8A62AD499B2B19EEDFE17D5E2617B4B5CAE604BC9CB3A0750C756386421731FB4AB1AC67E4A6D7A748DAFCB808DB914E9045D98829FA6868FCB766D0F1D5C89BAF2E449E878D552F2714C751317BC83E2B6B1C90DF07A2A55DEAB9926E0973A018B1065D93AB9026DE6C47294B9C60B773D93D1BA0FBE6CB6A947B49577BA4AC7BF4957B700EB0F519277A7AE2DDE69F61187DA69B3052130CD429C5968DA5E2141A9E89ACB0B84B2EA0444ED1B845246A7EC0C79B8CD34B94234AD715DA1169E8CD9562C4CCDB36D19F2410328E8FBF8BFE36D7B7DBB4DB7012CD6D9166F055C3C1C563FA043E4F674A0194D27FF06549F2615F3A432F936241C0D57A99C30CAA33901B7C3B5E61342B42B14EB3B6F3E20A8D09AA06779E3FCC42037B411AD1F9D7E0C96962FD4C59A132A75BC3C1CB717710F133D8AB94F272C3933E213F6A43F05B00EE6B3427E9404BB675E40360756C33C4D998206BCF4D5362B74550138346A005A800CAA2297EAB6B88B82C75DB0B65CD21D2A4DE67F32FFDE3957D81A6806A8B84D93C03409F43E09545843EDBFFA0D0632500B74B2FA1F839C5698D93E156FAA4D96DF7049F62026C302B41FA56AB8FF1AC47BEDD4D153D86671DA2870DF34E8F06532762764EC2C1E9E3C76D98005D5270BA3E37510D709ED6FB171F4395C6FF399D3F56508A83EE148EB917A7F1A0206607A1B629A2F2B894D8F435EC284A95D1D148F438082E0370540B17B42BFC45504BB12B821C9AFE9F6460469F6F09D56D754F6E2E2765AD5696D6F9B3BB61834603F35C5C961D5D5B11DD09A843E84A75A0A72C6FA469726BBA6A9D2E95E2F1C42173F647A5772FC377AEBB19A9E86BCECE9D772E6F56896A9898763CA6D7BA7795F2117B9575B81768B2A4BDA63B282176BEC72A35DAE3B59E263BD7D588FD364815FBA05D65AA77B595F51ABA49621AD1128E8EAEDEAEF757BB5ACD46C6134C59D2CAB9B4D9DAC69C7F3FB9E6DA97818E3664D1BF40D73203F19EF6337DEBA5C189D6C3865F748636FBD63B1F98761B362F38FFBD61E22DB13C2777A43422CE4C5E9CD29BA59E7BCE264A03917C7AB5505A95C7E3200834025C3F0930F792EC6617B39145BD6D18D578ED88B97A179174F1E7AE7290538EC9D9BF372B0DBF2C36A0EA0A62C74A2E8360738E5F6506B4FB381A33E8F7A1A2D65D2F0D3100B451F84A3F9DAC0B48E7861EB08DE51B0B7F343D2549B8F1C79867BBFC62ED81E16E0EFD2B771F09836B2E2D9F0826615FF3A956F51CA765C2CE76ACEF3E15D85BBF8398783A876F2987E088B0C5C55176F56EBC2672E6F995ECFCF0100A4C277CF6916AE9BD21750A807F1893FDEA4E9661995DD6A2E072559F82DFB6B186FEB60F579A17C8864CE6F92D5AC7A30435468D1D05E53856573BDC845176D7361E5237E3DFF1DE8238353B34C6C391DDAAC103F3F3BBB00F47352617123210AE29C419A0F5F946470A28D9265B40D62665394FACCA9BA18A58693FAE5A7701B26C52CCB943BA7096505BC1D0D3BC5993089EB6A2160CA0035340D3639FAFA9CD802C89ABBFFF2E85FA822B9BA4DF2693FD7F7D94DB958CC7904E9325841439BEBCECAAE5D082485EF66D4BBA15227A22120A9130587BFF87A6714488AD1691A1C51238F15C6002945D0E19B3B943C86AB21F0EE84475D0F0680A36E7C4E028D8684B31470B8D9675B0829998BF9186566AE1558C1B4AEA639DA5960A2DEF13A8086B1F22E262C0416CF5C8CAFD2C69E0CA0D7C631E3B44109CF378A7EDF6E1DF2575118732186811CAF608376A796208AA097C3D1A946976E0FA0355DF0C1699E96D0B1A958B3D56B8961984C6000C581B9565BA6ED9EB5C76954934D956A3A27B5AAB9D53AE13032B2221C60922E8F72E226DC3078F8FCEC1BC0491076EAAC1716FC14277CB6FE76582C1B39DC72823B5A9C16D1A99EC65CFD220956D850D2194827F070780D6517B5790D0CCD36243900526A6F30582BB63E4582C00A8DAB7F6C6E15A753C369BF761839CD50F3BB8CA9E89AC4165C8869B25CB021DDD3C6AC4512A52EF3AA175093621C01D9A49C2C96D5E243EB51377241E475D36E2B1D861DEED05457DAECB774C910EEE266130C097EA4A699ECCE807BBCD4B0719A20E7983802B872362E19F1F7BD03F694372E8D3D191CAC27BC7169C8B1608526E4510E13B8FDFA0DC6C4571D5C767F0806D21B0BC6404036586E5E858F82E676CDCADDB8216BD86C761996BB340F2FFB321DE443A51F608B8B22E07DABD0C44F378919525F1CDBFC66DBE701EC84EDB05BCD7E38A9A3B1216A9E046BDDA82BF6AD130D1FB32E80DC1BBDDA188DEF8B15EE4F4E27E8ECEA3A318AE29F9E8B8B8793A75063882D0F2F6B1D928258DC7DD586A53F89DB60DA2E0C804AED1871F88F7F1F4C8D6E4DA1850E752DEC2FD559092CAE315101B24F027F54EB07801E351E27813A7EC4400A370EE1035B40218F52F8887588D668C1D9DF69B7753B0700ADF5A871DAA489603B2AB87591C84CE062852583B01AF838CC2AB66527F5EBA4010C610E887D86B038ADC1E20F8F0A77103CC4841A3A920884CAC080A60355C1A6714F403A419892D580B8A5646203D6D1412A850930A1008F19001150067AB17720F0B0340363BF132AB11E0C88486C7C4E0A8D64DC391374CC41E8B876AA67436A8E43CA77623C9B54930C0704B2494A36A01EF9E08B1100C0681719D1001023ECB0316B114CC0C67D3D22036DECD890E6DA38AE36381F7A9DF7A68C1F51BC42CE6BB40112DE7F28CE2DAA87F34884885FD2B00A129156213854301664EFC24C7C3E9FCE676F9A701572F4018065B93A160D0190C20A31E836CFE9718ACD3B7413A96AF309A5D3ECF4198808DBF9181DE924C5404A7AFA8711539E681AC8C1C7A880202C62A0D9DA3640ABFD64A0413D250214A9824EF4692118CA332502DFE290128245791813E3C7534013A6021E5121C43045B375830C24D12BF7802A5A8AA96575345052CF2AD7D2A469F0FA29543658866B0A0CB72B682361A868C71F9C689BF8820A4CFB599C39E96CE8E138D03424875304D4ECD5873906124D84238C8890B1D4A8D1201321A2CAA08C812AB2F50AC9220E1D93AC4E8DE17286499456377529C724A823C5267288C24BD229F740B8A4DA1444343D6A5E13DC339DDB5345989A09C549EF078B4625AD138CF1A89A9EE21E18704F2D025009A46BDF505D33C822E1880B8D9284C9CA1C4E49EE8D36A09228A5D609D409471B0749117ADB8FCEF24143F620E23187F691BAA30DEE23F446766E35F2D186F3E148DB413686003288946C42CE48DD63069D113AAAFAF21AD931C3CC08C48D6E7B57614AAB16931CC94B6D9A5E6277DB7C480FBBD7C6C4B383D09C429C20F2EC1E2A451249A7602982B478EBAFEEAC9181B25B93791BB976AE670F12FE8A9C2314F09ABC17D18377E40217D2B5B117A826AA06224B6E0C0EA9838C281C8CBE71496AC4655CBF3BCFF3C6601BF4D46F17A7039BBCD9913AA041151735662F811D9BC386910FA1EBD49F193E42DF5F9DCA7714E5407AAE8D12418B8D115502EB9E3EAE04149EB00A36CB4E1F494220CED9B0EA2E4964FCCDE234C436D0769B8E6EE04BB074508261341B3E90D72C9DF48FE9D1D50EF99C1E7139EBAD14C6228A7C402F7AC8C6DDC9AE5233AC01988FBA351D34AC019C0537EC1AC0F05A982B39E2C685B99BF0C6854719C25B1216A6A18B17C9717C982F5B71878FE3DCD8B990637931EC07AC0C11F21EBF6AFB6F7CFEDA55C0C6F7AE98AEDB1DACF81B84E685A485F0F157952C99807795BE840D1E52D242369D22F9102E3E25999F591AFA8A4F448E821B64F6C11FF269F6CF352FFED00D6FFCCD1FB2A35B1D0232F6D0F1577E7D6D12ABAFCAB0ED4CEDCB3379B7917A7B26BA66CD49A66EDB927A6DD6931CF8EF9C1001393E92426FB8F19F490992C00E4F35C2B57F1865C5AB83F8752F713482673FE041C5C079C2830880B7E6B3797933949461F6585AB4FAC7226857C9E72248FF78C2235F7920140967BE83B8E48C85B4A8E8270B68A7D0470B48870EE7F90C29A1CF143832EF201B3A213B2D27DE657AB48BC6EBF47C40D8D1D788D3E7028F71295B0740E6556E1C3DE6CBDC1838DB1B221C849AAF6FFBB281751EB9E61E70F3ED6A71B77C0AD741F5C3D5222FB20CB7D93E88CB849D69FDE143B0DD46C963DAD6AC7E99DD6D8365E1B1FDFE6E3EFBB68E93F47AFE9465DB578B455A924ECFD6D172B749375FB2B3E566BD08569BC5E5F9F91F1717178BF581C66229E15DBDB5DC70CA36BBE03154BE16F72856E1DB689766C51AE2212872DEBD5EAD4131E5D63371EBA766265D6C86A3595F00AA8B17FFAF2F36B6C9F20A7E67E83D9D56806FF33EADF3012DBB17AA176760BDBCE6DD3288831D92BCF4F526DEAF13FAFE3B5DFB6D1497ED9769B4BFDA517AF32D0B9322312024277CE2D3BCDD458F5112C4782BE1578BD6EEE3F863903D290D6D7EE553FA10ADC3C3F25DA4D4FE0A295D2D1408A8305B009C291AAFA2968569E4AE57277C9BE931C0CE21D20FF2FF163ECBD5CB1FF8F53F4759AC0C7AF5139F46D1EDCFE5530C914CFB2B9FD24D9DC85024744365371C15839A9B7996E8AB1F71B8C18EACDD0FDE7E490F214C440AF56F7C2A6F72DF2CBE59AD76619ACAB4E42F7C8A627815919E2EEC8A4646524A64495AD2177B8A3F3EE3F47EB4D25A29D3B13414E2076B7A6AE3849F8F47F79AADA26E8A47EC8071B48EACDA8FCAD5FCA0EB207F71A0B829979028C90DBAA2A769E6563A7A4CC2D5CD6EF91465E13203465CFD6C413B5F957D55FA5EFF3629F50B506AE9ACA0935E6B0E4418AAADADDD8F7617BCA066B7BF4E53E0A42D8AB62877C63BE98BFCD0D65E630CF5FBD11925B0A848C8107374B431633CC1B018351335C6C09949F433760DDF9FF7EB87E294523605CA4717BAC0982A9F2C70B6D965B7BB95DA4AE1E7A3C197E68EAE05AE282A0C3CD1558FCF061869D251C9513EFC20E64EBC1137DD5CDA997371ABC5CC512835F9142FC0A7209EE174B2263C9A0CDBC225D48FA53166449676F36DD3271FD9887B714FAC483B8FFFC08E0BDE08E43C4753AE5FD783A2F56B94460FEA2E7FF3E3D100927C74E6C5A53151B57071CCA4FA4120E00FC147149970A7C11DE76591FD0E14796183BF0DA521D1D34E73CDF07DF8358C955D24E51B9F2A9D1D5BA4CFCFA1CD683FB2552E7FB2F68ECBEDE6288B08B758FC6C4D5B0C278DD0D6459BD62061F2B58FD5D7663C84B2B7374DC82367734353E8C7DAA829B4E52B2CFAF4DA34D59B380B7749501C0E412B003EDAAF929B50AFD8E2988C03AB91E1A4A7C7AAA7D86BE84EAACA20C8505716957E55166A96FC65F23735B0529EAA7A39EE3C04A7733FF024EAF774E42925EA968E3DB529BC698A7D1B7D9F9BCC93C13F5A830F430D743B5B3591E39CAF9A69F4A9A4509BC4DF2733AF0113F345B617F36FC7C3625EB025DCF3591FDE1A0852568517790E6C78A2EE136D06DAF62833121C045D752B8CA8920B4EB650834AF9E97C27104AA1B6ED21A7AFDE0FC090DD54EB4DD4E2755002575DCDAF7DBEF4A1377697BF1D025FC81BB9F5AF7694903B07C2CFD335C8C95B57BDF53AFA443727BD0AB9EFE09A5335272B62B5D03888111A12E983353DC49CC85F268B325914C5A20869353AD994360387BD55D1D4EDC7AE340CA1C3AB7C72A0F96B10EF29A2D5378BB7AAC0DED8DA9A49F78E56F7609E994E2A6824C75045068D7E54F2C018EAA3F8FB8BDCBD60057DB24081317A0A03050C1AFDA0A0CFDB38684647781D8795F891D10308E5A3BC8FA3C6015285827D9F269F1730F970025839989D0E7772CC244EE5841F2649C6546ABA3AC3A6774A0AE5E592839282CF5D99C6B8E60015C9ED04B5EE03A54093EA30E89D92EA78529A0EEA3264F4363FBB9275CB919D02E98BC3EC05B627E52F93EABD20D52BE3B47AD1BE2245ABBB02A2B5FB3D04AFB2DC92979B3B3C6FD03E6E70A02BAF812F50F2A04C472E970C2E97C7B29CD5DC82615F97E97891C27881C2F9FD9BE6DD5BFF6ED5D8F6A909D5EBC748B9477D3093E8D75C61F10F8027612CDC09E2467E6D2147A8631BC1E0B33DE4C9BDBF8EBB7E938333BE832307D356EE4D18B24973A3452155ADA2431551C5D525332B41349426CFC2A8E4305817826D5AD5B9C155B875D706DBB6EFF5265945C598CFDEA53FEFE3F87AFE2588532A1A122D063518BB35C46EB786A4C3D6C12384BA6ED14190C13370EA08B7868C0798995A7A8C38D3F4BF33BE74399819B120B06ADC7856C8E898732B7784504BCF0396CCC9A58F12463A1974C693BA8EE3592A4DAD0E6822A9761C1E40D70396E8B6769CAA7B81124302FE2C1388E1C1324CB0966D6C109DC603017443544DD7A751024D3C4620D11DF78E1F985ED30246B0B28F598E20DE71A428AA3D808BCA217A5418E3CBC38C399041492DD22C47AB5F9ABF9B0C4A55F62229AD5229A02249522998B4CAA4A4A6333A1499CF72697C8D56652AA3E7340BD7D51CFAF7F8751C9596AE2EF02148A22F619A7DDEFC1626D7F3CBF38BCBF9EC268E82F49024AB4AD4F46A595EBF0E9264935529B418999B2EBE2F323785ABF542AD6E9FFFA9A092A6AB18C9FE540C5505172411D2D5DF42008A1A2CF9AA6C46E1EA6AA156BC42B059F0BE9E47CDE4F1973029560FE1EA6390152FE18B5261D9CAF9AC805FF05024F6AA20B8D0926F73141D98245F83DDF229D8FDDB3AF8F6EF22B56C07AFF662C4847C4A1E28C2544A3E9AD96453F240AC7D20614D4CDC12D202CE948CE864D157A63692E5369FA96AFCEA5DB20ABF5DCFFF2B37AEB9D1FCFB3E7C35FB9C0B73F6DFF3D987E0DBFB3079CC9EAEE717E7E7D6FC3F1F1223A92DA0A97210D1A649F200AF265592300AB4808A3AAF66EFFEE3BEAAF6DDACBC0FFC6A765E08CB1B16D10C45270BC23AD9910E05977FB01D3839ED918EF60FF6B815DF29D901A3AD496183D50079DFFFD08462733C8B0A1BED48EC47C4187414947400D0B199C2F67FA756F2550D4D4B74B27A26E7361AD1EA2B1991348379F983B5F547D22279C6749D27E940F6A138719FB4F7F8B4570827F54214B84D61C47798A6E96C5208EC64F585A8841233CE0EBC52E58EF8454E80ED1A8390E861F5A0CF5174B23000598E045ECEA4C6769384E7A4DCEEB07180EF529FECF81F8919300449EBD0348A605FCDEDC535C76EF1F5C167F2267CB612BF2E6087638C067F762391249F3B3BA99648C2438BD0C3A54E3A2F50EAC11FE0DCF43AD95901EF9C9039CA6E600CE43A9962E96A979B6E0D099097E53DE27D3C45FF0F126922577277ABAC5D487DEEA693050591F769C2038507FD35AA93C5811A17C65DF4743E263B834FD1E9E2BE28C1633CEF2F226FEEF44EAAEBD2420C23E3E475B504A6FDCCA35B81B8FB46BDB846DA244F276BEFD43C517602976B77D2219064848F2A9B055B1325C76D9556559FACC5B19F7E18733C9DBCC64E0E2AEF4018095974B2632FA777B2336162DD53B0D4A39FF14D56BAFF336A7D72A613D7D3C94233AFEC58E4483A5948B0322A9D16565C075A9BA6E8A50CB09CDCE8B40676482340E7133A592474DE3A6C23F7F14FCA5984A9D7339D41D6E62C3A10CEE93E4449B07B06AF30D8E47AB92B305D7DFCA7702BB1C4429339391D7322652EF26251E4D4459351998C8AAD5121F20A9DAC5951721379F2501D50A1E633D229A7CBA353C140B9DD109E94AB77E532640A3A592513130E9DD61A903D7486F43E273B74BE6F82A041415DAE822084FE99EF82603985ECC40A294CDED8B14F18FAF43E276B74F487BB5DC36F8054416E7A32DD8238352D798187E0BD6A88BB6E4C5A713A5AF142F481B5BDD7451BBAAEA0E5443F5EB6F3263D19504F40169D93551525138F2C46A78061200B8F3F9A20F54E5FA42F7D92A6577A5DA8E29975FCD33F64D4F14057C9A7E381E2C0BE89BD9578A13106A89C37FD6C94C09C377C3ED6B8EC8381876D2E8B0DAEC959D0D3EACD5910222637FB886AD4D47B2C72EF9B6435FBB429B81DBE56CD2BE2129F55BF7CD8C759B48DA365CEF77A7E7E7606D2CE09549060AD2249ECB34CFF7780785E302C365AA320CEABA7D92E7716407EA18FBB285946DB20967AA394621ABB42CC0D3DF5CB4FE1364C0A2B467798C3B5896C0A59371C140B6C92831437DB008EEAA0FB5E88679A92C86822314A23D9FC288FDF0508BD7E9BE45634CCC2D9CDF210D6FA75902E8315D49922C6B80E5775E0551550CDEFBD20098F43D91F98D0F0B2043BF1AAC32840122EF8DFE3823A321C897109C56648BF9F3C8EC8E88BC78A234322366A34F1646FD590C28F5633991CB04F24AC7CE9052D7A11F4831A6D763B9C212F05DDE000A2A38F76B2043DD9A47191666B2B060799F2026D1478DD6ED18475F8CF26ABA5AF258EBFA1E420E8C4DBC068664F78B590793F08267ACBE1AC6DFBB1819BD8416A91212795ABB0D0FE6835DB1E1BCA3439A48F095086A49C0380479383D2841B10BB0AC3112C64852B2B8C7AC68E3E36573F58B2C22D371FE7700BC93620241E67C9E4CD29E90965974EF8688521B5FA216997057DBF9E1A992AB1C795A4DC770E5F3ADED671E0CB38BBF589A831ACD218E8B1B246CC84AEC381A53E4596E22A9178C142DC88C38B7E1F72CF4B3D141781DC7EEB057BC6F03FFDC04F1B238C60A946E03A0A00AAAA41EFD57718EB7ED1E76647FDDA3E2B20789E386D4CA078777ED46DFDF2628E10AA88841D8C1C236D63C1AF836EF64BF78BA44DBEEA4B2FA03344D3E91576D80D6382A11CC4EA08E0765A9BB7E3626CE0CD5B7B741DC3E62D0AAFDAC61AA752DB917DB14E9CFDE08FE0BE81C0A2E36E8AB1F72F7CEEA51E83C735E4FEA993AB2586353E0A8C50A1C8CC73A03E5E1B3A3D1AAA0C62D146DC04B18A71D70F685DB64674A9AD8E0AC375B4352E76D510741ACC82A2A7B2F1C60AB7777C589372291D09C84E6261302AD486590EB8E16AC4958018DDF034EE2B4AF118B1761C3E9CFC8D453AEE24C16FFC2B8B871066A701A33A049FB42B57FF76F2E041030C1E2B6EC0731A2A8E0C092858521C56E4AB958B04EA83E36E23074F103248A41F2CE1DDE730D6C4151A1568CD2912681ABDF705E2B960E33FF0415247D87B06E59067496E8A8045621A15884AC41A33FAD0011E186DC8F6AFFAA9579C0D892F8BAD5DF1F9F3A8A0FAB4F9C73D2EAB23055411BE016B46F9FB8B8112085271EC38AA4E8754A3693652D606A2677C394DD99E9136DC1991D3740C23AF8D6BC2EA0DAEC196007D9B386CEB0B7E7C798B06AB0DB1B1560A6FCA0802C553E3BC46B86BE205ACC2B7D12ECD8A4DE9872085936951EB2ECCC407ECF3D99B261C811C34E06EF914AE83EBF9EA6193A3E010CFA0FA04569E32612C4400E08215C25862E518FC9B17E528E3E62BC5B1296062D56C02413ECD279449F3D5C041DAF6064CA4AF181FA9808195F256113053BE63EC94220686F07D2FE0098B606C612903E7D684018EED278C53FBD5C0817AEB04F8510531EE5459A7B6D0423794E7B7CC7638E0532172786051DD70C1D23CB513E65F4AF784221A05144AF138B77E1EC5B82DA1E1DB1632B0452FB703D668298C3D5A9069EA2A9F9B3476D5779DB9AB8A980C1EBCD50A8D1E2C831A3E588C6BD80D3739689B6FA8A89D0E0C75EDDA0E4EF44D6D0615186D05759853F3E1788D9C9E0F9F7553F4A184094AF5510C0450FD05854DFDD140BECD95021908DF3016C267A31556734660E61794C1ED2E2866608EAC780073A40CC61C29C664AE31BAB0888E35DBECAA1B0D245F9DD553CB30796AB8E9F9B039943B622493F2AB8E4F5980CB8AF6E860112D53CAC3131663BAB54E15A46D261427973C584037693D2EADD172AEC4124C3EAD26975F2501C6826A2177952306341C19260373DC32B933CA02EBD00562F104C4A0AC029BFE53CB3BFB8EA3E1B3907E9BC36C79EA36B25E2C2BEB1682F6DD36447B420460131F4AEA10B5482C7B655AF84994D0F56D4946BF6CED2A1E49F426C9682E80751BE59144E11477079152F7F83D9200584BE25220562B5B060F06F1FEC4DE4E6E6C09533BA2E84549A16BD43C7A3422D2C43D41A4C38D92820A86DAD7900465DAA37017B9FBBC668CE6414F7576814030EB069602A289239D7B2DA5C34D280B6A3E44A7533A66548B9EC433108AF0300CB43418611BA46E68F6AECA1E3136A33001ABAB4151BEE402AFBB9060A80086A40CF1057C75CF1F0E3BF8DAE04DBBC6DFD6BF7F977D226A37F0E0179936F850FF4C5AD54B1E1ABE54EF2A1483ABC97CA6EDD5D51C5914ED3510AE3C888B233E3A35A295A11FB4EA9C1D8B49DBDD091CDA8EB0DF6D3224C37BF3892B146BE35FD635AB6DFB41A77AE343420B61E28F0F7542244E2030E119CE1146161A6EB7CD0FE8BC9AECBEBB8D3FE2D2EC166A5E7BF9DA2D444E8BA4DAE82990C3CE90FAE608DB10D2BE4BF2D461E5C0EAE0E310A751F6DDE43F9141FAEFF8BEC674BD4EE8A8F184484F0BAC22CD274EEE22D43DFED0088FFD6604ED2AEA94980EB77C0E40078181470A1A29E91F34A01D42FBC11205E22D1A4EEEDC85205DAAD70880BE7CEFB9F3C251A054193BE373EF3679075C2302DEBD713F23EA4FD9BA6083BEADAC430AF38E73EF6618F54E8CA7B7B4B8EA8C5CCD7DDAE6DBD5E270505CFD90FF996D76C163F861B30AE3B4FCF56AF1699F1409C50E7FE5AE55F4D892B8CA692661397E2DD1BACCBBE4CBA6BE53ACB4A82EA2E420FB1066C12AF7596F7659F4255866D57E78943CCE67BF06F13E2FF266FD10AEDE25B7FB6CBBCF8AB3E2F543FC2C0AA3B88EACE37FB5006DBEBADD960AE1A30B7933A32291DA6DF2E33E8A574DBBDF2299D20812C53DE72A2F61319659919FF0F1B9A1F4738E241EA14A7CCDF5EC1A43E96D72177C0D5DDAF64B1ABE0F1F83E573FEFBD76855E09022621E0859EC579577965634DAFAF99F398657EB6F7FFA7F603E800590290200 , N'6.1.3-40302')